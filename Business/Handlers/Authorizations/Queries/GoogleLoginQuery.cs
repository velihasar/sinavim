using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Services.Authentication;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Google;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Business.Handlers.Authorizations.Queries
{
    public class GoogleLoginQuery : IRequest<IDataResult<AccessToken>>
    {
        public string IdToken { get; set; }

        [JsonPropertyName("kvkkAccepted")]
        public bool KvkkAccepted { get; set; }

        public string AuthorizationCode { get; set; }
    }

    public class GoogleLoginQueryHandler : IRequestHandler<GoogleLoginQuery, IDataResult<AccessToken>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICacheManager _cacheManager;
        private readonly IConfiguration _configuration;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public GoogleLoginQueryHandler(
            IUserRepository userRepository,
            ITokenHelper tokenHelper,
            ICacheManager cacheManager,
            IConfiguration configuration,
            IGroupRepository groupRepository,
            IUserGroupRepository userGroupRepository)
        {
            _userRepository = userRepository;
            _tokenHelper = tokenHelper;
            _cacheManager = cacheManager;
            _configuration = configuration;
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
        }

        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<AccessToken>> Handle(GoogleLoginQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
            {
                return new ErrorDataResult<AccessToken>("Google ID token gereklidir.");
            }

            GoogleUserInfo googleUser;
            try
            {
                googleUser = await GoogleTokenValidator.ValidateTokenAsync(request.IdToken, _configuration);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ErrorDataResult<AccessToken>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new ErrorDataResult<AccessToken>(ex.Message);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<AccessToken>($"Google ile giriş yapılırken bir hata oluştu: {ex.Message}");
            }

            if (string.IsNullOrWhiteSpace(googleUser.Email))
            {
                return new ErrorDataResult<AccessToken>("Google hesabınızda e-posta adresi bulunamadı.");
            }

            var normalizedEmail = googleUser.Email.Trim().ToLower();
            var emailForDb = googleUser.Email.Trim();
            if (emailForDb.Length > 50)
            {
                emailForDb = emailForDb.Substring(0, 50);
            }

            User user = null;

            if (!string.IsNullOrWhiteSpace(googleUser.GoogleId))
            {
                user = await _userRepository.GetAsync(u =>
                    u.GoogleId == googleUser.GoogleId && u.Status);
            }

            if (user == null)
            {
                user = await _userRepository.GetAsync(u =>
                    u.Email != null &&
                    u.Email.Trim().ToLower() == normalizedEmail &&
                    u.Status);
            }

            if (user != null && !user.Status)
            {
                return new ErrorDataResult<AccessToken>(Messages.EmailNotVerified);
            }

            if (user == null)
            {
                var pending = await _userRepository.GetAsync(u =>
                    u.Email != null &&
                    u.Email.Trim().ToLower() == normalizedEmail &&
                    !u.Status);
                if (pending != null)
                {
                    return new ErrorDataResult<AccessToken>(Messages.EmailNotVerified);
                }
            }

            if (user == null)
            {
                if (!request.KvkkAccepted)
                {
                    return new ErrorDataResult<AccessToken>(
                        "KVKK Aydınlatma Metni'ni kabul etmelisiniz.");
                }

                var fullName = !string.IsNullOrWhiteSpace(googleUser.Name)
                    ? googleUser.Name.Trim()
                    : normalizedEmail.Split('@')[0];

                user = new User
                {
                    Email = emailForDb,
                    FullName = fullName.Length > 100 ? fullName.Substring(0, 100) : fullName,
                    GoogleId = googleUser.GoogleId,
                    Status = true,
                    PasswordHash = null,
                    PasswordSalt = null,
                    BirthDate = new DateTime(2000, 1, 1),
                    Gender = 0,
                };

                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync();

                var ogrenci = _groupRepository.Get(g => g.GroupName == StudentGroupConstants.GroupName);
                if (ogrenci != null && user.UserId > 0)
                {
                    var already = await _userGroupRepository.GetAsync(
                        ug => ug.UserId == user.UserId && ug.GroupId == ogrenci.Id);
                    if (already == null)
                    {
                        _userGroupRepository.Add(new UserGroup
                        {
                            UserId = user.UserId,
                            GroupId = ogrenci.Id,
                        });
                        await _userGroupRepository.SaveChangesAsync();
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(user.GoogleId) && !string.IsNullOrEmpty(googleUser.GoogleId))
                {
                    user.GoogleId = googleUser.GoogleId;
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                }
            }

            var claims = _userRepository.GetClaims(user.UserId);
            var accessToken = _tokenHelper.CreateToken<DArchToken>(user);
            accessToken.Claims = claims.Select(x => x.Name).ToList();

            user.RefreshToken = accessToken.RefreshToken;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _cacheManager.Add($"{CacheKeys.UserIdForClaim}={user.UserId}", claims.Select(x => x.Name));

            return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
        }
    }
}
