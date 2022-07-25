namespace ml_ht
{
    public class HeadTurn : MelonLoader.MelonMod
    {
        HeadTurner m_localTurner = null;

        public override void OnApplicationStart()
        {
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        void OnPlayerJoined(VRC.Player p_player)
        {
            if(p_player == Utils.GetLocalPlayer())
                OnRoomJoined();
        }

        void OnRoomJoined()
        {
            m_localTurner = Utils.GetLocalPlayer().gameObject.AddComponent<HeadTurner>();
        }

        void OnRoomLeft()
        {
            m_localTurner = null;
        }
    }
}
