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
    /// <summary>Anasayfa rekabet kartı için hafif özet (son 7 gün soru sıralaması + bugün).</summary>
    public class GetArkadasRekabetDashboardOzetQuery : IRequest<IDataResult<ArkadasRekabetDashboardOzetDto>>
    {
        public class GetArkadasRekabetDashboardOzetQueryHandler
            : IRequestHandler<GetArkadasRekabetDashboardOzetQuery, IDataResult<ArkadasRekabetDashboardOzetDto>>
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

            public GetArkadasRekabetDashboardOzetQueryHandler(
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
            public async Task<IDataResult<ArkadasRekabetDashboardOzetDto>> Handle(
                GetArkadasRekabetDashboardOzetQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<ArkadasRekabetDashboardOzetDto>("Oturum bulunamadı.");
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
                    7,
                    cancellationToken);

                var ozet = ArkadasRekabetHelper.BuildDashboardOzet(rekabet, userId);
                return new SuccessDataResult<ArkadasRekabetDashboardOzetDto>(ozet);
            }
        }
    }
}
