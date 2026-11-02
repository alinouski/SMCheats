namespace SMCheats
{
    public interface ICheatsPanelController
    {
        string Id { get; }
        int Order { get; }
        CheatsBuiltInTabs BuiltInFlag { get; }

        void OnRegistered(CheatsServiceLocator serviceLocator);
        void OnSelected();
        void OnDeselected();
        void OnUnregistered();
    }
}