using System;
using Business.Handlers.KullaniciDersNetHedefis.Queries;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.KullaniciDersNetHedefis.Queries.GetKullaniciDersNetHedefiQuery;
using static Business.Handlers.KullaniciDersNetHedefis.Queries.GetKullaniciDersNetHedefisQuery;
using static Business.Handlers.KullaniciDersNetHedefis.Commands.CreateKullaniciDersNetHedefiCommand;
using Business.Handlers.KullaniciDersNetHedefis.Commands;
using Business.Constants;
using static Business.Handlers.KullaniciDersNetHedefis.Commands.UpdateKullaniciDersNetHedefiCommand;
using static Business.Handlers.KullaniciDersNetHedefis.Commands.DeleteKullaniciDersNetHedefiCommand;
using MediatR;
using FluentAssertions;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class KullaniciDersNetHedefiHandlerTests
    {
        Mock<IKullaniciDersNetHedefiRepository> _kullaniciDersNetHedefiRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _kullaniciDersNetHedefiRepository = new Mock<IKullaniciDersNetHedefiRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task KullaniciDersNetHedefi_GetQuery_Success()
        {
            var query = new GetKullaniciDersNetHedefiQuery();

            _kullaniciDersNetHedefiRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciDersNetHedefi, bool>>>())).ReturnsAsync(new KullaniciDersNetHedefi()
            {
                Id = 1,
                UserId = 1,
                DersId = 2,
                HedefNet = 35m,
            });

            var handler = new GetKullaniciDersNetHedefiQueryHandler(_kullaniciDersNetHedefiRepository.Object, _mediator.Object);

            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
            x.Data.UserId.Should().Be(1);
            x.Data.DersId.Should().Be(2);
        }

        [Test]
        public async Task KullaniciDersNetHedefi_GetQueries_Success()
        {
            var query = new GetKullaniciDersNetHedefisQuery();

            _kullaniciDersNetHedefiRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<KullaniciDersNetHedefi, bool>>>()))
                        .ReturnsAsync(new List<KullaniciDersNetHedefi> {
                            new KullaniciDersNetHedefi { Id = 1, UserId = 1, DersId = 1, HedefNet = 10 },
                            new KullaniciDersNetHedefi { Id = 2, UserId = 1, DersId = 2, HedefNet = 20 },
                        });

            var handler = new GetKullaniciDersNetHedefisQueryHandler(_kullaniciDersNetHedefiRepository.Object, _mediator.Object);

            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            x.Success.Should().BeTrue();
            x.Data.Should().HaveCount(2);
        }

        [Test]
        public async Task KullaniciDersNetHedefi_CreateCommand_Success()
        {
            var command = new CreateKullaniciDersNetHedefiCommand
            {
                UserId = 1,
                DersId = 5,
                HedefNet = 40m,
            };

            _kullaniciDersNetHedefiRepository.Setup(x => x.Query())
                .Returns(new List<KullaniciDersNetHedefi>().AsQueryable());

            _kullaniciDersNetHedefiRepository.Setup(x => x.Add(It.IsAny<KullaniciDersNetHedefi>())).Returns(new KullaniciDersNetHedefi());

            var handler = new CreateKullaniciDersNetHedefiCommandHandler(_kullaniciDersNetHedefiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciDersNetHedefiRepository.Verify(v => v.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Data.Should().NotBeNull();
            x.Data.UserId.Should().Be(1);
            x.Data.DersId.Should().Be(5);
        }

        [Test]
        public async Task KullaniciDersNetHedefi_CreateCommand_NameAlreadyExist()
        {
            var command = new CreateKullaniciDersNetHedefiCommand
            {
                UserId = 1,
                DersId = 5,
                HedefNet = 40m,
            };

            _kullaniciDersNetHedefiRepository.Setup(x => x.Query())
                                           .Returns(new List<KullaniciDersNetHedefi> {
                                               new KullaniciDersNetHedefi { Id = 9, UserId = 1, DersId = 5, HedefNet = 1 },
                                           }.AsQueryable());

            var handler = new CreateKullaniciDersNetHedefiCommandHandler(_kullaniciDersNetHedefiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task KullaniciDersNetHedefi_UpdateCommand_Success()
        {
            var command = new UpdateKullaniciDersNetHedefiCommand
            {
                Id = 1,
                UserId = 1,
                DersId = 5,
                HedefNet = 42m,
            };

            _kullaniciDersNetHedefiRepository.Setup(x => x.Query())
                .Returns(new List<KullaniciDersNetHedefi> {
                    new KullaniciDersNetHedefi { Id = 1, UserId = 1, DersId = 5, HedefNet = 40m },
                }.AsQueryable());

            _kullaniciDersNetHedefiRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciDersNetHedefi, bool>>>()))
                        .ReturnsAsync(new KullaniciDersNetHedefi() { Id = 1, UserId = 1, DersId = 5, HedefNet = 40m });

            _kullaniciDersNetHedefiRepository.Setup(x => x.Update(It.IsAny<KullaniciDersNetHedefi>())).Returns(new KullaniciDersNetHedefi());

            var handler = new UpdateKullaniciDersNetHedefiCommandHandler(_kullaniciDersNetHedefiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciDersNetHedefiRepository.Verify(v => v.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Data.HedefNet.Should().Be(42m);
        }

        [Test]
        public async Task KullaniciDersNetHedefi_DeleteCommand_Success()
        {
            var command = new DeleteKullaniciDersNetHedefiCommand { Id = 1 };

            _kullaniciDersNetHedefiRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciDersNetHedefi, bool>>>()))
                        .ReturnsAsync(new KullaniciDersNetHedefi() { Id = 1, UserId = 1, DersId = 5 });

            _kullaniciDersNetHedefiRepository.Setup(x => x.Delete(It.IsAny<KullaniciDersNetHedefi>()));

            var handler = new DeleteKullaniciDersNetHedefiCommandHandler(_kullaniciDersNetHedefiRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciDersNetHedefiRepository.Verify(v => v.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
