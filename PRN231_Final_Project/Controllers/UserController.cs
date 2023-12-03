using AutoMapper;
using AutoMapper.Execution;
using Azure.Core;
using BusinessObjects;
using BusinessObjects.DTOs;
using BusinessObjects.Helper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN231_Final_Project.Helper;
using Repositories.Interfaces;
using Repositories.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231_Final_Project.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IRefreshTokenRepository _rfTokenRepo;
        private readonly IConfiguration _configuration;
        private readonly Prn231PrjContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly FirebaseApp _app;

        public UserController(IConfiguration configuration, IUserRepository repository, IRefreshTokenRepository rfTokenRepo, Prn231PrjContext context, IMapper mapper, IMemoryCache memoryCache, FirebaseApp app)
        {
            _configuration = configuration;
            _repository = repository;
            _rfTokenRepo = rfTokenRepo;
            _context = context;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _app = app;
        }
        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<CustomResponse> SignIn(string username, string password)
        {
            try
            {
                var user = _repository.SignIn(username, password);
                if (user == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "User does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                var uDto = _mapper.Map<UserDTO>(user);
                var token = await GenerateToken(user);
                if (token == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Add refresh token fail",
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Success = false
                    };
                }
                return new CustomResponse
                {
                    Data = new
                    {
                        token,
                        user = uDto
                    },
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        private async Task<TokenModel> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Username", user.Username),
                    new Claim("Id", user.UserId.ToString()),

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accesToken = jwtTokenHandler.WriteToken(token);
            _memoryCache.Set("accessToken", accesToken, DateTime.Now.AddDays(1));
            var refreshToken = GenerateRefreshToken();
            //save to db
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.UserId,
                IsUsedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(7)
            };
            if (_rfTokenRepo.AddRefreshToken(refreshTokenEntity) <= 0)
            {
                return null;
            }

            return new TokenModel
            {
                AccessToken = accesToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<CustomResponse> RenewToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]);
            var tokenValidateParam = new TokenValidationParameters
            {
                // Disabling issuer and audience validation
                ValidateIssuer = false,
                ValidateAudience = false,

                // Enabling issuer signing key validation
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false // ko kiem tra het han

            };

            try
            {
                //check if accessToken valid format 
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

                //check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals
                        (SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return new CustomResponse
                        {
                            Data = { },
                            Message = "Invalid token",
                            StatusCode = (int)HttpStatusCode.OK,
                            Success = false
                        };
                    }

                }
                //check if accessToken expired
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x =>
                x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Access Token has not yet expired",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = false
                    };
                }
                //check refreshToken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(
                    x => x.Token == model.RefreshToken);
                if (storedToken == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Refresh Token does not exist",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = false
                    };
                }
                //check refreshToken is used/revoked?
                if ((bool)storedToken.IsUsed)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Refresh Token has been used",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = false
                    };
                }
                if ((bool)storedToken.IsRevoked)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Refresh Token has been revoked",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = false
                    };
                }
                //check accessToken id == jwtId in Refresh token
                var jti = tokenInVerification.Claims.
                    FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Token doesn't match",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = false
                    };
                }
                //update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();

                //create new token
                var user = await _context.Users.SingleOrDefaultAsync(
                    x => x.UserId == storedToken.UserId);
                var token = await GenerateToken(user);
                return new CustomResponse
                {
                    Data = token,
                    Message = "Renew token successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                return new CustomResponse
                {
                    Data = { },
                    Message = "Something went wrong",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<CustomResponse> ForgotPassword(string email)
        {
            try
            {
                string code = Helper.Helper.GenerateCode();
                bool IsSent = await SendPasswordResetEmail(email, code);
                if (!IsSent)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Sent email fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
                // save code to cache and email
                _memoryCache.Set("resetPassCode", code, DateTime.Now.AddMinutes(10));
                _memoryCache.Set("resetPassEmail", email, DateTime.Now.AddMinutes(30));

                return new CustomResponse
                {
                    Data = { },
                    Message = "Check your email to get reset code",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        private async Task<bool> SendPasswordResetEmail(string email, string code)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Support", ""));
                emailMessage.To.Add(new MailboxAddress("", email));

                emailMessage.Subject = "Password Reset Code";

                var builder = new BodyBuilder();
                builder.HtmlBody = $"<b>Password Reset Code: {code}</b>";

                emailMessage.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("herehola113@gmail.com", "hthn uktz nrqg hhbm");
                await smtp.SendAsync(emailMessage);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [AllowAnonymous]
        [HttpPost("validate-reset-code")]
        public async Task<CustomResponse> ValidateResetCode(string code)
        {
            try
            {
                // Lấy ra từ cache
                string correctCode = (string)_memoryCache.Get("resetPassCode");
                // remove from cache
                _memoryCache.Remove("resetPassCode");
                if (correctCode.IsNullOrEmpty() || !correctCode.Equals(code))
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Reset code is not correct",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Reset code is correct",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [AllowAnonymous]
        [HttpPut("reset-password")]
        public async Task<CustomResponse> ResetPassword(string password)
        {
            try
            {
                // Lấy ra từ cache
                string email = (string)_memoryCache.Get("resetPassEmail");
                if (email.IsNullOrEmpty())
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Reset password fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
                var user = _repository.GetSingleUserByEmail(email);
                // remove email from cache
                _memoryCache.Remove("resetPassEmail");
                if (user == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "User does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                string newHashedPassword = PasswordHelper.HashPassword(password);
                user.Password = newHashedPassword;
                if (_repository.UpdateUser(user) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Reset password fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
                return new CustomResponse
                {
                    Data = { },
                    Message = "Reset password successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };

            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        // GET: api/<UserController>
        [HttpGet]
        [Authorize]
        public CustomResponse GetAllUsers(string? searchValue)
        {
            try
            {
                List<User> listUser = _repository.GetUsers(searchValue);
                List<UserDTO> listUserDto = _mapper.Map<List<UserDTO>>(listUser);
                return new CustomResponse
                {
                    Data = listUserDto,
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        // GET api/<UserController>/id
        [HttpGet("{id}")]
        [Authorize]
        public CustomResponse GetSingleUser(int id)
        {
            try
            {
                var user = _repository.GetSingleUser(id);
                var uDto = _mapper.Map<UserDTO>(user);

                if (user == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "User does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                return new CustomResponse
                {
                    Data = uDto,
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };

            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [HttpGet("by-username")]
        [Authorize]
        public CustomResponse GetSingleUserByUsername(string username)
        {
            try
            {
                var user = _repository.GetSingleUserByUsername(username);
                var uDto = _mapper.Map<UserDTO>(user);

                if (user == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "User does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                return new CustomResponse
                {
                    Data = uDto,
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };

            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [AllowAnonymous]
        // POST api/<UserController>/register
        [HttpPost("register")]
        public CustomResponse Post(UserCreateDTO uDto)
        {
            try
            {
                if (_repository.GetSingleUserByUsername(uDto.Username) != null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "This username is existed!",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                } else if (_repository.GetSingleUserByEmail(uDto.Email) != null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "This email is existed!",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
                uDto.Password = PasswordHelper.HashPassword(uDto.Password);
                var user = _mapper.Map<User>(uDto);
                if (_repository.Register(user) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Register fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
                return new CustomResponse
                {
                    Data = { },
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Success = false
                };
            }
        }

        // PUT api/<UserController>/change-password/id
        [HttpPut("change-password/{id}")]   
        [Authorize]
        public CustomResponse Put(int id, string oldPassword, string newPassword)
        {
            try
            {
                var user = _repository.GetSingleUser(id);
                if (user == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "User does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                if (!PasswordHelper.VerifyPassword(oldPassword, user.Password))
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Old password is not correct",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
                else
                {
                    string newHashedPassword = PasswordHelper.HashPassword(newPassword);
                    if (_repository.ChangePassword(id, newHashedPassword) <= 0)
                    {
                        return new CustomResponse
                        {
                            Data = { },
                            Message = "Changed password fail",
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Success = false
                        };
                    }
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Changed password successfully",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        // PUT api/<UserController>/id
        [HttpPut("{id}")]
        [Authorize]
        public CustomResponse Put(int id, UserUpdateDTO uDto)
        {
            try
            {
                var user = _repository.GetSingleUser(id);
                if (user == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "User does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                uDto.Password = user.Password;
                uDto.Avatar = user.Avatar;
                var u = _mapper.Map<User>(uDto);
                if (_repository.UpdateUser(u) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Update user fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Update user's profile successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };

            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [HttpPost("upload-image/{userId}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<CustomResponse> UploadFile([FromRoute] int userId, IFormFile file)
        {
            try
            {
                    if (file == null || file.Length == 0)
                    {
                        return new CustomResponse
                        {
                            Data = { },
                            Message = "No file uploaded",
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Success = false
                        };
                    }

                    string fileType = Helper.Helper.GetFileType(file); // Validate and get type of file
                    if (fileType != "image")
                    {
                        return new CustomResponse
                        {
                            Data = { },
                            Message = "Only image is accepted",
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Success = false
                        };
                    }

                    if (file.Length > Helper.Helper.MaxFileSize(fileType)) // Validate file size
                    {
                        return new CustomResponse
                        {
                            Data = { },
                            Message = "File size is too large, image's max size is 5MB",
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Success = false
                        };
                    }

                    // Initialize Firebase Storage
                    var storage = new FirebaseStorage("nmt-insta.appspot.com");

                    // Generate a unique file name (e.g., using a GUID)
                    string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

                    // Upload the file to Firebase Storage
                    var path = $"media/{uniqueFileName}";
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                    await storage.Child(path).PutAsync(stream);
                }

                // Return the URL of the uploaded file in the response
                var mediaUrl = await storage.Child(path).GetDownloadUrlAsync();
                
                if (HandleUpdateAvatar(userId, mediaUrl) <= 0) // update avatar to db
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Update avatar to database fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
                return new CustomResponse
                {
                    Data = new { mediaUrl },
                    Message = $"Upload image successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }
        // handle update avatar to db
        private int HandleUpdateAvatar(int userId, string mediaUrl)
        {
            try
            {
                var user = _repository.GetSingleUser(userId);
                if (user == null)
                {
                    return 0;
                }
                user.Avatar = mediaUrl;
                return _repository.UpdateUser(user);

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
