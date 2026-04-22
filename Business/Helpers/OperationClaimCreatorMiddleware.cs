using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Fakes.Handlers.Authorizations;
using Business.Fakes.Handlers.Groups;
using Business.Fakes.Handlers.OperationClaims;
using Business.Fakes.Handlers.UserClaims;
using Core.Utilities.IoC;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Helpers
{
    public static class OperationClaimCreatorMiddleware
    {
        public static async Task UseDbOperationClaimCreator(this IApplicationBuilder app)
        {
            var mediator = ServiceTool.ServiceProvider.GetService<IMediator>();
            foreach (var operationName in GetOperationNames())
            {
                await mediator.Send(new CreateOperationClaimInternalCommand
                {
                    ClaimName = operationName
                });
            }

            var operationClaims = (await mediator.Send(new GetOperationClaimsInternalQuery())).Data;
            var user = await mediator.Send(new RegisterUserInternalCommand
            {
                FullName = "System Administrator",
                Password = "Adana.14531989",
                Email = "no-reply@masavtech.com",
            });
            await mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = 1,
                OperationClaims = operationClaims
            });

            await mediator.Send(new EnsureStudentGroupAndClaimsInternalCommand());
        }

        /// <summary>
        /// [SecuredOperation] ile işaretlenmiş handler'ların dış Command/Query sınıf adları (ör. GetKonuTakipForMeQuery).
        /// </summary>
        private static IEnumerable<string> GetOperationNames()
        {
            var names = new HashSet<string>();
            var handlerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t =>
                    t.Namespace != null &&
                    t.Namespace.StartsWith("Business.Handlers", System.StringComparison.Ordinal) &&
                    t.IsNested &&
                    t.Name.EndsWith("Handler", System.StringComparison.Ordinal) &&
                    t.DeclaringType != null &&
                    (t.DeclaringType.Name.EndsWith("Command", System.StringComparison.Ordinal) ||
                     t.DeclaringType.Name.EndsWith("Query", System.StringComparison.Ordinal)));

            foreach (var handlerType in handlerTypes)
            {
                var handleMethod = handlerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(m =>
                        m.Name == "Handle" &&
                        m.GetCustomAttributes(typeof(SecuredOperation), inherit: true).Length > 0);
                if (handleMethod != null)
                {
                    names.Add(handlerType.DeclaringType!.Name);
                }
            }

            return names;
        }
    }
}
