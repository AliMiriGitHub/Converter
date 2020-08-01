using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Services.Utility;

namespace Services
{
    public class ConverterService : Converter.ConverterBase
    {

        [Authorize]
        public override Task<ConvertReply> ConvertToWord(ConvertRequest request, ServerCallContext context)
        {
            var converter = new NumberToWordsConverter();
            return Task.FromResult(new ConvertReply
            {
                Description = converter.Convert(request.Amount)
            });
        }
    }
  
}
