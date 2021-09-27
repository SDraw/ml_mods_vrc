namespace ml_ht
{
    public class HeadTurn : MelonLoader.MelonMod
    {
        HeadTurner m_localTurner = null;

        public override void OnApplicationStart()
        {
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(CreateLocalTurner());
        }
        System.Collections.IEnumerator CreateLocalTurner()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            m_localTurner = Utils.GetLocalPlayer().prop_VRCPlayer_0.gameObject.AddComponent<HeadTurner>();
        }

        void OnRoomLeft()
        {
            m_localTurner = null;
        }
    }
}
