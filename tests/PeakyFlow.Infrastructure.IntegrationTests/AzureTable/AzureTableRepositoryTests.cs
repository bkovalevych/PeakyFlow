using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameRoleAggregate;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Abstractions.RoomStateAggregate;
using PeakyFlow.Infrastructure.AzureTable;
using PeakyFlow.Infrastructure.AzureTable.Mappings;
using PeakyFlow.Infrastructure.AzureTable.Models;
using PeakyFlow.Infrastructure.Redis;
using PeakyFlow.Infrastructure.Redis.Models;

namespace PeakyFlow.Infrastructure.IntegrationTests.AzureTable
{
    public class AzureTableRepositoryTests
    {
        private readonly Mock<ILogger<RedisRepository<Room, RoomM>>> _fakeLogger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public AzureTableRepositoryTests()
        {
            _fakeLogger = new Mock<ILogger<RedisRepository<Room, RoomM>>>();
            _mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.AddExpressionMapping();
                x.AddProfile<AzureTableMappingProfile>();
            }));

            _configuration = new ConfigurationBuilder()
                .AddUserSecrets<AzureTableRepositoryTests>()
                .Build();

            _connectionString = _configuration["AzureTableConnectionString"];
        }


        [Fact]
        public async Task GivenRoom_WhenAdd_ThenAdded()
        {
            //Assign

            var rep = new AzureTableRepository<Room, RoomTableEntity>(new TableServiceClient(_connectionString), _mapper);
            await rep.Init();
            var result = await rep.AddAsync(new Room()
            {
                Id = "b5bcf98a-db17-44da-b2d9-ad15e07cdb35",
                Name = "Room1",
                Players =
                [
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc0",
                        Name = "BohdanChyk",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Active
                    },
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc1",
                        Name = "Dan",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Lost
                    }
                ]
            });
            await rep.DeleteAsync(result);
            // assert

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GivenGameMap_WhenAdd_ThenAdded()
        {
            //Assign

            var rep = new AzureTableRepository<GameMap, GameMapTableEntity>(new TableServiceClient(_connectionString), _mapper);
            await rep.Init();
            var result = await rep.AddAsync(new GameMap()
            {
                Id = "b5bcf98a-db17-44da-b2d9-ad15e07cdb35",
                GameMapPlayers =
                [
                    new GameMapPlayer()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc0",
                        Name = "BohdanChyk",
                        Level = 1,
                        Position = 0,
                        SkeepTurns = 0
                    },
                    new GameMapPlayer()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc1",
                        Name = "Dan",
                        Level = 1,
                        Position = 1,
                        SkeepTurns = 1
                    }
                ],
                Steps = [StepType.Deal, StepType.Market]
            });

            var list = await rep.ListAsync(default);

            await rep.DeleteAsync(result);
            // assert

            Assert.NotNull(result);
            Assert.Single(list);
        }

        [Fact]
        public async Task GivenRoomState_WhenAdd_ThenAdded()
        {
            //Assign


            var rep = new AzureTableRepository<RoomState, RoomStateTableEntity>(new TableServiceClient(_connectionString), _mapper);
            await rep.Init();

            var result = await rep.AddAsync(new RoomState()
            {
                Id = "b5bcf98a-db17-44da-b2d9-ad15e07cdb35",
                PlayerStates =
                [
                    new PlayerState()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc0",
                        Name = "BohdanChyk",
                        RoleName = "Lawer",
                        Description = "descr"

                    },
                    new PlayerState()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc1",
                        Name = "Dan",
                        RoleName = "Lawer",
                        Description = "desrc",
                        CountableLiabilities = [new CountableLiabilityItem("1", "Children", FinancialType.ChildrenExpenses, 1, 10, 100, "gr")],
                        FinancialItems = [new FinancialItem("2", "House2fu", FinancialType.RealEstate, 100, 90, 10, "ge")],
                        PercentableLiabilities = [new PercentableLiabilityItem("3", "Nam", FinancialType.Loan, 1000, 10)],
                        Savings = 100,
                        Stocks = [new StockItem("4", "fke", FinancialType.Stock, 10, 5)],
                        ImageId = "img"
                    }
                ],
                Indeces = { [CardType.BigDeal] = 1, [CardType.SmallDeal] = 3, [CardType.MoneyToTheWind] = 6 },
                Cards =
                {
                    [CardType.SmallDeal] = ["1", "2", "2"],
                    [CardType.BigDeal] = ["1b", "2b"],
                    [CardType.MoneyToTheWind] = ["1mw"],
                    [CardType.Market] = ["1m"],
                }
            });

            var list = await rep.ListAsync(default);

            await rep.DeleteAsync(result);
            // assert

            Assert.NotNull(result);
            Assert.Single(list);
        }
    }
}
