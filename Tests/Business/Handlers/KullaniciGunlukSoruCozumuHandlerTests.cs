
using Business.Constants;
using Business.Handlers.KullaniciGunlukSoruCozumus.Commands;
using Business.Handlers.KullaniciGunlukSoruCozumus.Queries;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Commands.CreateKullaniciGunlukSoruCozumuCommand;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Commands.DeleteKullaniciGunlukSoruCozumuCommand;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Commands.UpdateKullaniciGunlukSoruCozumuCommand;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Queries.GetKullaniciGunlukSoruCozumuQuery;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Queries.GetKullaniciGunlukSoruCozumusQuery;

namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class KullaniciGunlukSoruCozumuHandlerTests
    {
        Mock<IKullaniciGunlukSoruCozumuRepository> _kullaniciGunlukSoruCozumuRepository;
        Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _kullaniciGunlukSoruCozumuRepository = new Mock<IKullaniciGunlukSoruCozumuRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_GetQuery_Success()
        {
            var query = new GetKullaniciGunlukSoruCozumuQuery { Id = 1 };

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                .ReturnsAsync(
                    new KullaniciGunlukSoruCozumu
                    {
                        Id = 1,
                        UserId = 10,
                        Tarih = DateTime.UtcNow.Date,
                        CozulenSoruSayisi = 5,
                    });

            var handler = new GetKullaniciGunlukSoruCozumuQueryHandler(
                _kullaniciGunlukSoruCozumuRepository.Object,
                _mediator.Object);

            var x = await handler.Handle(query, default);

            x.Success.Should().BeTrue();
            x.Data.Should().NotBeNull();
            x.Data!.Id.Should().Be(1);
            x.Data.UserId.Should().Be(10);
            x.Data.CozulenSoruSayisi.Should().Be(5);
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_GetQueries_Success()
        {
            var query = new GetKullaniciGunlukSoruCozumusQuery();

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.GetListAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                .ReturnsAsync(
                    new List<KullaniciGunlukSoruCozumu>
                    {
                        new KullaniciGunlukSoruCozumu { Id = 1, UserId = 1, Tarih = DateTime.UtcNow.Date, CozulenSoruSayisi = 1 },
                        new KullaniciGunlukSoruCozumu { Id = 2, UserId = 1, Tarih = DateTime.UtcNow.Date.AddDays(-1), CozulenSoruSayisi = 2 },
                    });

            var handler = new GetKullaniciGunlukSoruCozumusQueryHandler(
                _kullaniciGunlukSoruCozumuRepository.Object,
                _mediator.Object);

            var x = await handler.Handle(query, default);

            x.Success.Should().BeTrue();
            var list = x.Data!.ToList();
            list.Count.Should().Be(2);
            list[0].Should().BeOfType<KullaniciGunlukSoruCozumuDto>();
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_CreateCommand_Success()
        {
            var command = new CreateKullaniciGunlukSoruCozumuCommand
            {
                UserId = 1,
                Tarih = DateTime.UtcNow.Date,
                CozulenSoruSayisi = 10,
            };

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.Query())
                .Returns(new List<KullaniciGunlukSoruCozumu>().AsQueryable());

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.Add(It.IsAny<KullaniciGunlukSoruCozumu>()))
                .Returns(new KullaniciGunlukSoruCozumu());

            var handler = new CreateKullaniciGunlukSoruCozumuCommandHandler(
                _kullaniciGunlukSoruCozumuRepository.Object,
                _mediator.Object);
            var x = await handler.Handle(command, default);

            _kullaniciGunlukSoruCozumuRepository.Verify(r => r.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_CreateCommand_NameAlreadyExist()
        {
            var command = new CreateKullaniciGunlukSoruCozumuCommand
            {
                UserId = 1,
                Tarih = DateTime.UtcNow.Date,
                CozulenSoruSayisi = 5,
            };

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.Query())
                .Returns(
                    new List<KullaniciGunlukSoruCozumu>
                    {
                        new KullaniciGunlukSoruCozumu { Id = 1, UserId = 1, Tarih = DateTime.UtcNow.Date, CozulenSoruSayisi = 1 },
                    }.AsQueryable());

            var handler = new CreateKullaniciGunlukSoruCozumuCommandHandler(
                _kullaniciGunlukSoruCozumuRepository.Object,
                _mediator.Object);
            var x = await handler.Handle(command, default);

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_UpdateCommand_Success()
        {
            var command = new UpdateKullaniciGunlukSoruCozumuCommand
            {
                Id = 1,
                UserId = 2,
                Tarih = DateTime.UtcNow.Date,
                CozulenSoruSayisi = 20,
            };

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                .ReturnsAsync(
                    new KullaniciGunlukSoruCozumu
                    {
                        Id = 1,
                        UserId = 1,
                        Tarih = DateTime.UtcNow.Date.AddDays(-1),
                        CozulenSoruSayisi = 1,
                    });

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.Update(It.IsAny<KullaniciGunlukSoruCozumu>()))
                .Returns(new KullaniciGunlukSoruCozumu());

            var handler = new UpdateKullaniciGunlukSoruCozumuCommandHandler(
                _kullaniciGunlukSoruCozumuRepository.Object,
                _mediator.Object);
            var x = await handler.Handle(command, default);

            _kullaniciGunlukSoruCozumuRepository.Verify(r => r.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_DeleteCommand_Success()
        {
            var command = new DeleteKullaniciGunlukSoruCozumuCommand { Id = 1 };

            _kullaniciGunlukSoruCozumuRepository
                .Setup(x => x.Get(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                .Returns(
                    new KullaniciGunlukSoruCozumu
                    {
                        Id = 1,
                        UserId = 1,
                        Tarih = DateTime.UtcNow.Date,
                        CozulenSoruSayisi = 1,
                    });

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.Delete(It.IsAny<KullaniciGunlukSoruCozumu>()));

            var handler = new DeleteKullaniciGunlukSoruCozumuCommandHandler(
                _kullaniciGunlukSoruCozumuRepository.Object,
                _mediator.Object);
            var x = await handler.Handle(command, default);

            _kullaniciGunlukSoruCozumuRepository.Verify(r => r.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
