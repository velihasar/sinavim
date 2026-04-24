using Business.Constants;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;

namespace Business.BusinessAspects
{
    /// <summary>
    /// Cache'teki operasyon adlarıyla yetki kontrolü. Liste yok veya boşsa (claim atanmamış kullanıcı) giriş yeterli kabul edilir;
    /// en az bir kayıt varsa yalnızca listedeki komut/sorgu adlarına izin verilir.
    /// </summary>
    public class SecuredOperation : MethodInterception
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheManager _cacheManager;


        public SecuredOperation()
        {
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            // HttpContext ve User kontrolü
            if (_httpContextAccessor?.HttpContext == null || _httpContextAccessor.HttpContext.User == null)
            {
                throw new SecurityException(Messages.AuthorizationsDenied);
            }

            var userId = _httpContextAccessor.HttpContext.User.Claims
                ?.FirstOrDefault(x => x.Type.EndsWith("nameidentifier") || 
                                      x.Type == ClaimTypes.NameIdentifier || 
                                      x.Type == "nameid" || 
                                      x.Type == "sub")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new SecurityException(Messages.AuthorizationsDenied);
            }

            // Operation name'i belirle (önce bunu yap ki UpdateUserCommand kontrolü yapabilelim)
            string operationName = null;

            // Önce invocation arguments'ten command/query tipini al (en güvenilir yöntem)
            if (invocation.Arguments != null && invocation.Arguments.Length > 0)
            {
                var firstArg = invocation.Arguments[0];
                if (firstArg != null)
                {
                    operationName = firstArg.GetType().Name;
                }
            }

            // Eğer yukarıdaki yöntem çalışmazsa, handler'ın implement ettiği IRequestHandler<TRequest, TResponse> interface'inden al
            if (string.IsNullOrEmpty(operationName))
            {
                var handlerType = invocation.TargetType;
                var requestHandlerInterface = handlerType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                         i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)));

                if (requestHandlerInterface != null)
                {
                    var genericArgs = requestHandlerInterface.GetGenericArguments();
                    if (genericArgs.Length > 0)
                    {
                        operationName = genericArgs[0].Name;
                    }
                }
            }

            // Son çare olarak, handler class adından türet
            if (string.IsNullOrEmpty(operationName))
            {
                var handlerType = invocation.TargetType;
                var handlerClassName = handlerType.Name;
                if (handlerClassName.EndsWith("Handler"))
                {
                    operationName = handlerClassName.Substring(0, handlerClassName.Length - "Handler".Length);
                }
                else
                {
                    operationName = handlerClassName;
                }
            }

            // Özel durumlar: Kullanıcı kendi profilini güncelliyorsa izin ver
            // Bu kontrolü cache kontrolünden önce yapıyoruz
            var allowedSelfUpdateOperations = new[]
            {
                "UpdateUserCommand",
                "ChangeEmailCommand",
                "CancelEmailChangeCommand",
                "UserChangePasswordCommand",
                "RequestEmailChangeCommand",
                "ConfirmEmailChangeCommand",
            };

            if (allowedSelfUpdateOperations.Contains(operationName) && invocation.Arguments != null && invocation.Arguments.Length > 0)
            {
                var command = invocation.Arguments[0];
                if (command != null)
                {
                    // Reflection ile UserId property'sini al
                    var userIdProperty = command.GetType().GetProperty("UserId");
                    if (userIdProperty != null)
                    {
                        var commandUserIdValue = userIdProperty.GetValue(command);
                        if (commandUserIdValue != null)
                        {
                            // userId string olarak alınıyor (claim'den), commandUserId ise int olabilir
                            // Her iki durumu da kontrol et
                            var commandUserIdString = commandUserIdValue.ToString();
                            if (commandUserIdString == userId)
                            {
                                // Kullanıcı kendi profilini güncelliyor, izin ver
                                return;
                            }

                            // Eğer userId int olarak parse edilebiliyorsa, int karşılaştırması da yap
                            if (int.TryParse(userId, out var userIdInt) &&
                                commandUserIdValue is int commandUserIdInt &&
                                userIdInt == commandUserIdInt)
                            {
                                // Kullanıcı kendi profilini güncelliyor, izin ver
                                return;
                            }
                        }
                    }
                }
            }

            var oprClaims = _cacheManager.Get<IEnumerable<string>>($"{CacheKeys.UserIdForClaim}={userId}");

            if (oprClaims == null || !oprClaims.Any())
            {
                return;
            }

            if (oprClaims.Contains(operationName))
            {
                return;
            }

            throw new SecurityException(Messages.AuthorizationsDenied);
        }
    }
}