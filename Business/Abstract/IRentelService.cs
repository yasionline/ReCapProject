using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IRentelService
    {
        IResult Add(Rentel rentel);
        IDataResult<List<Rentel>> GetAll();
    }
}
