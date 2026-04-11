
using Business.Handlers.Motivasyons.Commands;
using Business.Handlers.Motivasyons.Queries;
using Business.Constants;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.MotivasyonDtos;
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
using static Business.Handlers.Motivasyons.Commands.CreateMotivasyonCommand;
using static Business.Handlers.Motivasyons.Commands.DeleteMotivasyonCommand;
using static Business.Handlers.Motivasyons.Commands.UpdateMotivasyonCommand;
using static Business.Handlers.Motivasyons.Queries.GetMotivasyonQuery;
using static Business.Handlers.Motivasyons.Queries.GetMotivasyonsQuery;

namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class MotivasyonHandlerTests
    {
        Mock<IMotivasyonRepository> _motivasyonRepository;
        Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _motivasyonRepository = new Mock<IMotivasyonRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Motivasyon_GetQuery_Success()
        {
            var query = new GetMotivasyonQuery { Id = 1 };

            _motivasyonRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Motivasyon, bool>>>()))
                .ReturnsAsync(new Motivasyon { Id = 1, Kelime = "Test" });

            var handler = new GetMotivasyonQueryHandler(_motivasyonRepository.Object, _mediator.Object);

            var x = await handler.Handle(query, default);

            x.Success.Should().BeTrue();
            x.Data.Should().NotBeNull();
            x.Data!.Id.Should().Be(1);
            x.Data.Kelime.Should().Be("Test");
        }

        [Test]
        public async Task Motivasyon_GetQueries_Success()
        {
            var query = new GetMotivasyonsQuery();

            _motivasyonRepository
                .Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Motivasyon, bool>>>()))
                .ReturnsAsync(new List<Motivasyon> { new Motivasyon { Id = 1, Kelime = "test" } });

            var handler = new GetMotivasyonsQueryHandler(_motivasyonRepository.Object, _mediator.Object);

            var x = await handler.Handle(query, default);

            x.Success.Should().BeTrue();
            x.Data.Should().NotBeNull();
            x.Data!.Count().Should().Be(1);
            x.Data!.First().Should().BeOfType<MotivasyonListDto>();
        }

        [Test]
        public async Task Motivasyon_CreateCommand_Success()
        {
            var command = new CreateMotivasyonCommand { Kelime = "deneme" };

            _motivasyonRepository.Setup(x => x.Query()).Returns(new List<Motivasyon>().AsQueryable());
            _motivasyonRepository
                .Setup(x => x.Add(It.IsAny<Motivasyon>()))
                .Callback<Motivasyon>(e => {
                    e.Id = 5;
                })
                .Returns((Motivasyon e) => e);

            var handler = new CreateMotivasyonCommandHandler(_motivasyonRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, default);

            _motivasyonRepository.Verify(r => r.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
            x.Data.Should().NotBeNull();
            x.Data!.Kelime.Should().Be("deneme");
        }

        [Test]
        public async Task Motivasyon_CreateCommand_NameAlreadyExist()
        {
            var command = new CreateMotivasyonCommand { Kelime = "test" };

            _motivasyonRepository
                .Setup(x => x.Query())
                .Returns(new List<Motivasyon> { new Motivasyon { Id = 1, Kelime = "test" } }.AsQueryable());

            var handler = new CreateMotivasyonCommandHandler(_motivasyonRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, default);

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Motivasyon_UpdateCommand_Success()
        {
            var command = new UpdateMotivasyonCommand { Id = 1, Kelime = "güncel" };

            _motivasyonRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Motivasyon, bool>>>()))
                .ReturnsAsync(new Motivasyon { Id = 1, Kelime = "eski" });

            _motivasyonRepository.Setup(x => x.Update(It.IsAny<Motivasyon>())).Returns(new Motivasyon());

            var handler = new UpdateMotivasyonCommandHandler(_motivasyonRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, default);

            _motivasyonRepository.Verify(r => r.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
            x.Data!.Kelime.Should().Be("güncel");
        }

        [Test]
        public async Task Motivasyon_DeleteCommand_Success()
        {
            var command = new DeleteMotivasyonCommand { Id = 1 };

            _motivasyonRepository
                .Setup(x => x.Get(It.IsAny<Expression<Func<Motivasyon, bool>>>()))
                .Returns(new Motivasyon { Id = 1, Kelime = "deneme" });

            var handler = new DeleteMotivasyonCommandHandler(_motivasyonRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, default);

            _motivasyonRepository.Verify(r => r.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
