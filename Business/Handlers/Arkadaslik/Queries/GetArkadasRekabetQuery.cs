using Business.BusinessAspects;
using Business.Helpers;
using Core.Entities.Dtos.Project.ArkadaslikDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Queries
{
    /// <summary>Arkadaş rekabet sayfası: soru, deneme ve konu sıralamaları.</summary>
    public class GetArkadasRekabetQuery : IRequest<IDataResult<ArkadasRekabetDto>>
    {
        /// <summary>Dönem gün sayısı (7–30, varsayılan 7).</summary>
        public int GunSayisi { get; set; } = 7;

        public class GetArkadasRekabetQueryHandler
            : IRequestHandler<GetArkadasRekabetQuery, IDataResult<ArkadasRekabetDto>>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;
            private readonly IUserRepository _userRepository;
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IKullaniciGunlukSoruCozumuRepository _gunlukSoruRepository;
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IKullaniciKonuIlerlemeRepository _konuIlerlemeRepository;
            private readonly IKonuRepository _konuRepository;
            private readonly IDersRepository _dersRepository;
            private readonly ISinavRepository _sinavRepository;

            public GetArkadasRekabetQueryHandler(
                IArkadaslikRepository arkadaslikRepository,
                IUserRepository userRepository,
                IKullaniciSinavRepository kullaniciSinavRepository,
                IKullaniciGunlukSoruCozumuRepository gunlukSoruRepository,
                IDenemeSinaviRepository denemeSinaviRepository,
                IKullaniciKonuIlerlemeRepository konuIlerlemeRepository,
                IKonuRepository konuRepository,
                IDersRepository dersRepository,
                ISinavRepository sinavRepository)
            {
                _arkadaslikRepository = arkadaslikRepository;
                _userRepository = userRepository;
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _gunlukSoruRepository = gunlukSoruRepository;
                _denemeSinaviRepository = denemeSinaviRepository;
                _konuIlerlemeRepository = konuIlerlemeRepository;
                _konuRepository = konuRepository;
                _dersRepository = dersRepository;
                _sinavRepository = sinavRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ArkadasRekabetDto>> Handle(
                GetArkadasRekabetQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<ArkadasRekabetDto>("Oturum bulunamadı.");
                }

                var rekabet = await ArkadasRekabetHelper.BuildRekabetAsync(
                    _arkadaslikRepository,
                    _userRepository,
                    _kullaniciSinavRepository,
                    _gunlukSoruRepository,
                    _denemeSinaviRepository,
                    _konuIlerlemeRepository,
                    _konuRepository,
                    _dersRepository,
                    _sinavRepository,
                    userId,
                    request.GunSayisi,
                    cancellationToken);

                return new SuccessDataResult<ArkadasRekabetDto>(rekabet);
            }
        }
    }
}
