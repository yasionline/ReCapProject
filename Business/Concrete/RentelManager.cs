using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class RentelManager : IRentelService
    {
        IRentelDal _rentelDal;
        public RentelManager(IRentelDal rentelDal)
        {
            _rentelDal = rentelDal;
        }
        public IResult Add(Rentel rentel)
        {
            _rentelDal.Add(rentel);
            return new SuccessResult("Rentel eklendi");
        }

        public IDataResult<List<Rentel>> GetAll()
        {
            return new SuccesDataResult<List<Rentel>>(_rentelDal.GetAll(), "Rentel Listelendi");
        }
    }
}
