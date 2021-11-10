namespace ml_ht
{
    public class HeadTurn : MelonLoader.MelonMod
    {
        HeadTurner m_localTurner = null;

        public override void OnApplicationStart()
        {
            GameUtils.Initialize(this.HarmonyInstance);
            GameUtils.OnRoomJoined += this.OnRoomJoined;
            GameUtils.OnRoomLeft += this.OnRoomLeft;
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(CreateLocalTurner());
        }
        System.Collections.IEnumerator CreateLocalTurner()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            m_localTurner = Utils.GetLocalPlayer().gameObject.AddComponent<HeadTurner>();
        }

        void OnRoomLeft()
        {
            m_localTurner = null;
        }
    }
}
