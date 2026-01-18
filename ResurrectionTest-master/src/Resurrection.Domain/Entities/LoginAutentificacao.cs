namespace Resurrection.Domain.Entities
{
    public class LoginAutentificacao
    {
        public ResponseDatasAuth ResponseDatasAuth { get; private set; }
        public ResponseDatasLogin ResponseDatasLogin { get; private set; }

        public void AtribuiResponseDatasAuth(ResponseDatasAuth responseDatasAuth)
            => ResponseDatasAuth = responseDatasAuth;

        public void AtribuiResponseDatasLogin(ResponseDatasLogin responseDatasLogin)
            => ResponseDatasLogin = responseDatasLogin;
    }
}
