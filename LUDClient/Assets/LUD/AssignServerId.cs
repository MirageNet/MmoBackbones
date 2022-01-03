using LUD.Authenticators;

namespace LUD
{
    public class AssignServerId : Mirage.ServerIdGenerator
    {
        #region Overrides of ServerIdGenerator

        public override byte GenerateServerId()
        {
            return GetComponent<ShardServerAuthenticator>().ShardId;
        }

        #endregion
    }
}
