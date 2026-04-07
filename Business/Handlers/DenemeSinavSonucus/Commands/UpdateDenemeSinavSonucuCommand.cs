
using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.DenemeSinavSonucus.ValidationRules;
using Core.Entities.Dtos.Project.DenemeSinavSonucuDtos;
using Core.Extensions;

namespace Business.Handlers.DenemeSinavSonucus.Commands
{


    public class UpdateDenemeSinavSonucuCommand : IRequest<IDataResult<UpdateDenemeSinavSonucuDto>>
    {
        public int Id { get; set; }
        public int DersId { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public int BosSayisi { get; set; }
        public decimal ToplamNet { get; set; }

        public class UpdateDenemeSinavSonucuCommandHandler : IRequestHandler<UpdateDenemeSinavSonucuCommand, IDataResult<UpdateDenemeSinavSonucuDto>>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;

            public UpdateDenemeSinavSonucuCommandHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDenemeSinavSonucuValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateDenemeSinavSonucuDto>> Handle(UpdateDenemeSinavSonucuCommand request, CancellationToken cancellationToken)
            {
                var isThereDenemeSinavSonucuRecord = await _denemeSinavSonucuRepository.GetAsync(u => u.Id == request.Id);

                if(isThereDenemeSinavSonucuRecord == null)
                {
                    return new ErrorDataResult<UpdateDenemeSinavSonucuDto>("Kayıt bulunamadı");
                }

                isThereDenemeSinavSonucuRecord.DersId = request.DersId;
                isThereDenemeSinavSonucuRecord.DogruSayisi = request.DogruSayisi;
                isThereDenemeSinavSonucuRecord.YanlisSayisi = request.YanlisSayisi;
                isThereDenemeSinavSonucuRecord.BosSayisi = request.BosSayisi;
                isThereDenemeSinavSonucuRecord.ToplamNet = request.ToplamNet;
                isThereDenemeSinavSonucuRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereDenemeSinavSonucuRecord.UpdatedDate = System.DateTime.Now;


                _denemeSinavSonucuRepository.Update(isThereDenemeSinavSonucuRecord);
                await _denemeSinavSonucuRepository.SaveChangesAsync();

                var dto = new UpdateDenemeSinavSonucuDto
                {
                    Id = isThereDenemeSinavSonucuRecord.Id,
                    DersId = isThereDenemeSinavSonucuRecord.DersId,
                    DogruSayisi = isThereDenemeSinavSonucuRecord.DogruSayisi,
                    YanlisSayisi = isThereDenemeSinavSonucuRecord.YanlisSayisi,
                    BosSayisi = isThereDenemeSinavSonucuRecord.BosSayisi,
                    ToplamNet = isThereDenemeSinavSonucuRecord.ToplamNet
                };

                return new SuccessDataResult<UpdateDenemeSinavSonucuDto>(dto, Messages.Updated);
            }
        }
    }
}

