
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.DenemeSinavis.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DenemeSinaviDtos;
using Core.Extensions;

namespace Business.Handlers.DenemeSinavis.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDenemeSinaviCommand : IRequest<IDataResult<CreateDenemeSinaviDto>>
    {

        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }
        public System.DateTime Tarih { get; set; }


        public class CreateDenemeSinaviCommandHandler : IRequestHandler<CreateDenemeSinaviCommand, IDataResult<CreateDenemeSinaviDto>>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;
            public CreateDenemeSinaviCommandHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDenemeSinaviValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateDenemeSinaviDto>> Handle(CreateDenemeSinaviCommand request, CancellationToken cancellationToken)
            {
                var addedDenemeSinavi = new DenemeSinavi
                {
                    Ad = request.Ad,
                    Aciklama = request.Aciklama,
                    UserId = request.UserId,
                    SinavId = request.SinavId,
                    SinavBolumId = request.SinavBolumId,
                    Tarih = request.Tarih.ToNpgsqlTimestamp(),
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = DateTimeExtensions.NowForNpgsqlTimestamp(),
                    IsActive = true
                };

                _denemeSinaviRepository.Add(addedDenemeSinavi);
                await _denemeSinaviRepository.SaveChangesAsync();
                
                var dto = new CreateDenemeSinaviDto
                {
                    Id = addedDenemeSinavi.Id,
                    Ad = addedDenemeSinavi.Ad,
                    Aciklama = addedDenemeSinavi.Aciklama,
                    UserId = addedDenemeSinavi.UserId,
                    SinavId = addedDenemeSinavi.SinavId,
                    SinavBolumId = addedDenemeSinavi.SinavBolumId,
                    Tarih = addedDenemeSinavi.Tarih
                };
                
                return new SuccessDataResult<CreateDenemeSinaviDto>(dto, Messages.Added);
            }
        }
    }
}