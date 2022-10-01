using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.Rest.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Murzik.Rest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FinDataSourceController : ControllerBase
    {
        private IReaderMurrProvider _readerMurrData;
        private IMapper _mapper;

        public FinDataSourceController(IReaderMurrProvider readerMurrProvider,
            IMapper mapper)
        {
            _readerMurrData = readerMurrProvider;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IReadOnlyCollection<FinDataSourcesView>> GetAll()
        {
            return (await _readerMurrData.GetFinDataSources())
                .Select(z => _mapper.Map<FinDataSourcesView>(z))
                .ToList();
        }
    }
}
