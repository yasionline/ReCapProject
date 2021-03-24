using Business.Abstract;
using Business.BusinessAspects.Autofac;
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

        [SecuredOperation("rental.add,moderator,admin")]
        public IResult Add(Rentel rentel)
        {
            _rentelDal.Add(rentel);
            return new SuccessResult("Rentel eklendi");
        }

        public IDataResult<List<Rentel>> GetAll()
        {
            return new SuccessDataResult<List<Rentel>>(_rentelDal.GetAll(), "Rentel Listelendi");
        }
    }
}
