
using Business.Handlers.KullaniciKonuIlerlemes.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.KullaniciKonuIlerlemes.Queries.GetKullaniciKonuIlerlemeQuery;
using Entities.Concrete;
using static Business.Handlers.KullaniciKonuIlerlemes.Queries.GetKullaniciKonuIlerlemesQuery;
using static Business.Handlers.KullaniciKonuIlerlemes.Commands.CreateKullaniciKonuIlerlemeCommand;
using Business.Handlers.KullaniciKonuIlerlemes.Commands;
using Business.Constants;
using static Business.Handlers.KullaniciKonuIlerlemes.Commands.UpdateKullaniciKonuIlerlemeCommand;
using static Business.Handlers.KullaniciKonuIlerlemes.Commands.DeleteKullaniciKonuIlerlemeCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class KullaniciKonuIlerlemeHandlerTests
    {
        Mock<IKullaniciKonuIlerlemeRepository> _kullaniciKonuIlerlemeRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _kullaniciKonuIlerlemeRepository = new Mock<IKullaniciKonuIlerlemeRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task KullaniciKonuIlerleme_GetQuery_Success()
        {
            //Arrange
            var query = new GetKullaniciKonuIlerlemeQuery();

            _kullaniciKonuIlerlemeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciKonuIlerleme, bool>>>())).ReturnsAsync(new KullaniciKonuIlerleme()
//propertyler buraya yazılacak
//{																		
//KullaniciKonuIlerlemeId = 1,
//KullaniciKonuIlerlemeName = "Test"
//}
);

            var handler = new GetKullaniciKonuIlerlemeQueryHandler(_kullaniciKonuIlerlemeRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.KullaniciKonuIlerlemeId.Should().Be(1);

        }

        [Test]
        public async Task KullaniciKonuIlerleme_GetQueries_Success()
        {
            //Arrange
            var query = new GetKullaniciKonuIlerlemesQuery();

            _kullaniciKonuIlerlemeRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<KullaniciKonuIlerleme, bool>>>()))
                        .ReturnsAsync(new List<KullaniciKonuIlerleme> { new KullaniciKonuIlerleme() { /*TODO:propertyler buraya yazılacak KullaniciKonuIlerlemeId = 1, KullaniciKonuIlerlemeName = "test"*/ } });

            var handler = new GetKullaniciKonuIlerlemesQueryHandler(_kullaniciKonuIlerlemeRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<KullaniciKonuIlerleme>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task KullaniciKonuIlerleme_CreateCommand_Success()
        {
            KullaniciKonuIlerleme rt = null;
            //Arrange
            var command = new CreateKullaniciKonuIlerlemeCommand();
            //propertyler buraya yazılacak
            //command.KullaniciKonuIlerlemeName = "deneme";

            _kullaniciKonuIlerlemeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciKonuIlerleme, bool>>>()))
                        .ReturnsAsync(rt);

            _kullaniciKonuIlerlemeRepository.Setup(x => x.Add(It.IsAny<KullaniciKonuIlerleme>())).Returns(new KullaniciKonuIlerleme());

            var handler = new CreateKullaniciKonuIlerlemeCommandHandler(_kullaniciKonuIlerlemeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciKonuIlerlemeRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task KullaniciKonuIlerleme_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateKullaniciKonuIlerlemeCommand();
            //propertyler buraya yazılacak 
            //command.KullaniciKonuIlerlemeName = "test";

            _kullaniciKonuIlerlemeRepository.Setup(x => x.Query())
                                           .Returns(new List<KullaniciKonuIlerleme> { new KullaniciKonuIlerleme() { /*TODO:propertyler buraya yazılacak KullaniciKonuIlerlemeId = 1, KullaniciKonuIlerlemeName = "test"*/ } }.AsQueryable());

            _kullaniciKonuIlerlemeRepository.Setup(x => x.Add(It.IsAny<KullaniciKonuIlerleme>())).Returns(new KullaniciKonuIlerleme());

            var handler = new CreateKullaniciKonuIlerlemeCommandHandler(_kullaniciKonuIlerlemeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task KullaniciKonuIlerleme_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateKullaniciKonuIlerlemeCommand();
            //command.KullaniciKonuIlerlemeName = "test";

            _kullaniciKonuIlerlemeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciKonuIlerleme, bool>>>()))
                        .ReturnsAsync(new KullaniciKonuIlerleme() { /*TODO:propertyler buraya yazılacak KullaniciKonuIlerlemeId = 1, KullaniciKonuIlerlemeName = "deneme"*/ });

            _kullaniciKonuIlerlemeRepository.Setup(x => x.Update(It.IsAny<KullaniciKonuIlerleme>())).Returns(new KullaniciKonuIlerleme());

            var handler = new UpdateKullaniciKonuIlerlemeCommandHandler(_kullaniciKonuIlerlemeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciKonuIlerlemeRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task KullaniciKonuIlerleme_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteKullaniciKonuIlerlemeCommand();

            _kullaniciKonuIlerlemeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciKonuIlerleme, bool>>>()))
                        .ReturnsAsync(new KullaniciKonuIlerleme() { /*TODO:propertyler buraya yazılacak KullaniciKonuIlerlemeId = 1, KullaniciKonuIlerlemeName = "deneme"*/});

            _kullaniciKonuIlerlemeRepository.Setup(x => x.Delete(It.IsAny<KullaniciKonuIlerleme>()));

            var handler = new DeleteKullaniciKonuIlerlemeCommandHandler(_kullaniciKonuIlerlemeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciKonuIlerlemeRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

