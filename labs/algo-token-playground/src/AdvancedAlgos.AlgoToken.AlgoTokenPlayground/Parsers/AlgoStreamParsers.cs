using System;
using System.Collections.Generic;
using System.Text;
using AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Commands.AlgoStreamContract;
using AdvancedAlgos.AlgoToken.Framework.Ethereum.Extensions;
using Sprache;

namespace AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Parsers
{
    public static class AlgoStreamParsers
    {
        public static void Register()
        {
            (from command in CommonParsers.Token("deploy-algostream")
             from minerType in CommonParsers.ByteValue
             from category in CommonParsers.ByteValue
             from minerAccountAddress in CommonParsers.StringValue
             from referralAccountAddress in CommonParsers.StringValue
             from tokenAddress in CommonParsers.StringValue
             from name in CommonParsers.Switch('n', "name", CommonParsers.Identifier).Optional()
             select new AlgoStreamDeployCommand
             {
                 Name = name.GetOrDefault(),
                 Size = minerType,
                 GracePeriodDays = category,
                 StreamHolderAddress = minerAccountAddress,
                 ReferralAddress = referralAccountAddress,
                 TokenAddress = tokenAddress
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-activatestream")
             select new AlgoStreamActivateStreamCommand
             {
                 ContractReference = contractReference,
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-pausestreaming")
             select new AlgoStreamPauseStreamingCommand
             {
                 ContractReference = contractReference,
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-resumestreaming")
             select new AlgoStreamResumeStreamingCommand
             {
                 ContractReference = contractReference,
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-disablestream")
             select new AlgoStreamDisableStreamCommand
             {
                 ContractReference = contractReference,
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-resetstream")
             from newStreamHolderAddress in CommonParsers.StringValue
             from newReferralAddress in CommonParsers.StringValue
             select new AlgoStreamResetStreamCommand
             {
                 ContractReference = contractReference,
                 NewStreamHolderAddress = newStreamHolderAddress,
                 NewReferralAddress = newReferralAddress
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-changestreamsize")
             from size in CommonParsers.ByteValue
             select new AlgoStreamChangeStreamSizeCommand
             {
                 ContractReference = contractReference,
                 Size = size
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-collect")
             select new AlgoStreamCollectCommand
             {
                 ContractReference = contractReference,
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-terminate")
             select new AlgoStreamTerminateCommand
             {
                 ContractReference = contractReference
             }).Register();

            (from contractReference in CommonParsers.Invoke("algostream-setcurrentdatetime")
             from value in CommonParsers.DateValue
             select new AlgoStreamSetCurrentDateTimeCommand
             {
                 ContractReference = contractReference,
                 Value = value.ToEpoch()
             }).Register();
        }
    }
}
