namespace ExViewer
{
    class Github
    {

        public const string BRANCH = "${Env:APPVEYOR_REPO_BRANCH}";

        public const string COMMIT = "${Env:APPVEYOR_REPO_COMMIT}";

    }
}