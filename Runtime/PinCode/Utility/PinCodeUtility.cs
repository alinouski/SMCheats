namespace SMCheats
{
    public static class PinCodeUtility
    {
        public static IPinCodeService CreateService(CheatsPinCodeSettings settings)
        {
            return settings is not { RequirePinCode: true } ? new MockPinCodeService() : new CheatsPinCodeService(settings);
        }
    }
}