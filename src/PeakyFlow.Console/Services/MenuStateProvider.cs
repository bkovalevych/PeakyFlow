using PeakyFlow.Console.Enums;
using PeakyFlow.GrpcProtocol.Lobby;
using Stateless;

namespace PeakyFlow.Console.Services
{
    public class MenuStateProvider
    {
        private readonly StateMachine<MenuState, MenuAction> _state;

        public event Action? OnStartMenu;

        public event Action? OnCreatingLobbyMenu;

        public event Action? OnEnterListLobbies;
        public event Action? OnExitListLobbies;

        public event Action<CreateLobbyMessage>? OnEnterLobbyMenuForOwner;
        public event Action? OnExitLobbyMenuForOwner;

        public event Action<JoinLobbyMessage>? OnEnterLobbyMenuForPlayer;
        public event Action? OnExitLobbyMenuForPlayer;

        public event Action? OnEnterGame;

        public event Action? OnRefreshLobbies;
        public event Action? OnRefreshLobbyForOwner;

        public StateMachine<MenuState, MenuAction>.TriggerWithParameters<JoinLobbyMessage> JoinLobbyTrigger { get; }
        public StateMachine<MenuState, MenuAction>.TriggerWithParameters<CreateLobbyMessage> CreateLobbyTrigger { get; }
        
        public MenuStateProvider()
        {
            _state = new StateMachine<MenuState, MenuAction>(MenuState.StartMenu);

            JoinLobbyTrigger = _state.SetTriggerParameters<JoinLobbyMessage>(MenuAction.JoinLobby);
            CreateLobbyTrigger = _state.SetTriggerParameters<CreateLobbyMessage>(MenuAction.CreateLobby);
            
            _state.Configure(MenuState.StartMenu)
                .OnActivate(() => OnStartMenu?.Invoke())
                .Permit(MenuAction.GoToListLobbies, MenuState.ListLobbiesMenu)
                .Permit(MenuAction.GoToCreateLobby, MenuState.CreatingLobbyMenu);

            _state.Configure(MenuState.CreatingLobbyMenu)
                .OnEntry(() => OnCreatingLobbyMenu?.Invoke())
                .Permit(MenuAction.GoToListLobbies, MenuState.ListLobbiesMenu)
                .Permit(MenuAction.CreateLobby, MenuState.LobbyMenuForOwner);

            _state.Configure(MenuState.ListLobbiesMenu)
                .OnEntry(() => OnEnterListLobbies?.Invoke())
                .OnExit(() => OnExitListLobbies?.Invoke())
                .InternalTransition(MenuAction.RefreshLobby, () => OnRefreshLobbies?.Invoke())
                .Permit(MenuAction.GoToCreateLobby, MenuState.CreatingLobbyMenu)
                .Permit(MenuAction.JoinLobby, MenuState.LobbyMenuForPlayer);


            _state.Configure(MenuState.LobbyMenuForOwner)
                .OnEntryFrom(CreateLobbyTrigger, x => OnEnterLobbyMenuForOwner?.Invoke(x))
                .OnExit(() => OnExitLobbyMenuForOwner?.Invoke())
                .Permit(MenuAction.CloseLobbyAndStartGame, MenuState.GameMenu)
                .Permit(MenuAction.CancellCreateLobby, MenuState.ListLobbiesMenu)
                .InternalTransition(MenuAction.RefreshLobbyForOwner, () => OnRefreshLobbyForOwner?.Invoke());

            _state.Configure(MenuState.LobbyMenuForPlayer)
                .OnEntryFrom(JoinLobbyTrigger, (arg) => OnEnterLobbyMenuForPlayer?.Invoke(arg))
                .OnExit(() => OnExitLobbyMenuForPlayer?.Invoke())
                .Permit(MenuAction.LeaveLobby, MenuState.ListLobbiesMenu)
                .Permit(MenuAction.StartGameForPlayer, MenuState.GameMenu);

            _state.Configure(MenuState.GameMenu)
                .OnEntry(() => OnEnterGame?.Invoke());

            
        }

        public void Fire(MenuAction action)
        {
            _state.Fire(action);
        }

        public void Fire<T>(StateMachine<MenuState, MenuAction>.TriggerWithParameters<T> trigger, T arg)
        {
            _state.Fire(trigger, arg);
        }

        public IEnumerable<MenuAction> AvailableActions()
        {
            return _state.GetPermittedTriggers();
        }


        public void Start()
        {
            _state.Activate();
        }

        public void Stop()
        {
            _state.Deactivate();
        }
    }
}
