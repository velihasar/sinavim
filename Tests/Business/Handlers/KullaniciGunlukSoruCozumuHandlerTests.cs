
using Business.Handlers.KullaniciGunlukSoruCozumus.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Queries.GetKullaniciGunlukSoruCozumuQuery;
using Entities.Concrete;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Queries.GetKullaniciGunlukSoruCozumusQuery;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Commands.CreateKullaniciGunlukSoruCozumuCommand;
using Business.Handlers.KullaniciGunlukSoruCozumus.Commands;
using Business.Constants;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Commands.UpdateKullaniciGunlukSoruCozumuCommand;
using static Business.Handlers.KullaniciGunlukSoruCozumus.Commands.DeleteKullaniciGunlukSoruCozumuCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Core.Entities.Concrete.Project;


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
            //Arrange
            var query = new GetKullaniciGunlukSoruCozumuQuery();

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>())).ReturnsAsync(new KullaniciGunlukSoruCozumu()
//propertyler buraya yazılacak
//{																		
//KullaniciGunlukSoruCozumuId = 1,
//KullaniciGunlukSoruCozumuName = "Test"
//}
);

            var handler = new GetKullaniciGunlukSoruCozumuQueryHandler(_kullaniciGunlukSoruCozumuRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.KullaniciGunlukSoruCozumuId.Should().Be(1);

        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_GetQueries_Success()
        {
            //Arrange
            var query = new GetKullaniciGunlukSoruCozumusQuery();

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                        .ReturnsAsync(new List<KullaniciGunlukSoruCozumu> { new KullaniciGunlukSoruCozumu() { /*TODO:propertyler buraya yazılacak KullaniciGunlukSoruCozumuId = 1, KullaniciGunlukSoruCozumuName = "test"*/ } });

            var handler = new GetKullaniciGunlukSoruCozumusQueryHandler(_kullaniciGunlukSoruCozumuRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<KullaniciGunlukSoruCozumu>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_CreateCommand_Success()
        {
            KullaniciGunlukSoruCozumu rt = null;
            //Arrange
            var command = new CreateKullaniciGunlukSoruCozumuCommand();
            //propertyler buraya yazılacak
            //command.KullaniciGunlukSoruCozumuName = "deneme";

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                        .ReturnsAsync(rt);

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.Add(It.IsAny<KullaniciGunlukSoruCozumu>())).Returns(new KullaniciGunlukSoruCozumu());

            var handler = new CreateKullaniciGunlukSoruCozumuCommandHandler(_kullaniciGunlukSoruCozumuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciGunlukSoruCozumuRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateKullaniciGunlukSoruCozumuCommand();
            //propertyler buraya yazılacak 
            //command.KullaniciGunlukSoruCozumuName = "test";

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.Query())
                                           .Returns(new List<KullaniciGunlukSoruCozumu> { new KullaniciGunlukSoruCozumu() { /*TODO:propertyler buraya yazılacak KullaniciGunlukSoruCozumuId = 1, KullaniciGunlukSoruCozumuName = "test"*/ } }.AsQueryable());

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.Add(It.IsAny<KullaniciGunlukSoruCozumu>())).Returns(new KullaniciGunlukSoruCozumu());

            var handler = new CreateKullaniciGunlukSoruCozumuCommandHandler(_kullaniciGunlukSoruCozumuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateKullaniciGunlukSoruCozumuCommand();
            //command.KullaniciGunlukSoruCozumuName = "test";

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                        .ReturnsAsync(new KullaniciGunlukSoruCozumu() { /*TODO:propertyler buraya yazılacak KullaniciGunlukSoruCozumuId = 1, KullaniciGunlukSoruCozumuName = "deneme"*/ });

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.Update(It.IsAny<KullaniciGunlukSoruCozumu>())).Returns(new KullaniciGunlukSoruCozumu());

            var handler = new UpdateKullaniciGunlukSoruCozumuCommandHandler(_kullaniciGunlukSoruCozumuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciGunlukSoruCozumuRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task KullaniciGunlukSoruCozumu_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteKullaniciGunlukSoruCozumuCommand();

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<KullaniciGunlukSoruCozumu, bool>>>()))
                        .ReturnsAsync(new KullaniciGunlukSoruCozumu() { /*TODO:propertyler buraya yazılacak KullaniciGunlukSoruCozumuId = 1, KullaniciGunlukSoruCozumuName = "deneme"*/});

            _kullaniciGunlukSoruCozumuRepository.Setup(x => x.Delete(It.IsAny<KullaniciGunlukSoruCozumu>()));

            var handler = new DeleteKullaniciGunlukSoruCozumuCommandHandler(_kullaniciGunlukSoruCozumuRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _kullaniciGunlukSoruCozumuRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

