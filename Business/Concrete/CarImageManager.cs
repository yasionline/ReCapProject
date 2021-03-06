﻿using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.FileSystems;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;
        public CarImageManager(ICarImageDal carImageDal)
        {
            _carImageDal = carImageDal;
        }

        //[SecuredOperation("carimage.add,moderator,admin")]
        [CacheRemoveAspect("ICarImageService.Get")]
        public IResult Add(CarImage carImage ,IFormFile file)
        {
            IResult result = BusinessRules.Run(CheckIfCarImageCountOfCarCorrect(carImage.CarId));
            if (result != null)
            {
                return result;
            }
            carImage.ImagePath = new FileManagerOnDisk().Add(file, CreateNewPath(file));
            carImage.Date = DateTime.Now;
            _carImageDal.Add(carImage);
            return new SuccessResult(Messages.CarImageAdded);
        }

        //[SecuredOperation("carimage.delete,moderator,admin")]
        [ValidationAspect(typeof(CarImageValidator))]
        [CacheRemoveAspect("ICarImageService.Get")]
        public IResult Delete(CarImage carImage)
        {
            new FileManagerOnDisk().Delete(carImage.ImagePath);
            _carImageDal.Delete(carImage);
            return new SuccessResult(Messages.CarImageDeleted);
        }

        public IDataResult<CarImage> GetById(int id)
        {
            return new SuccessDataResult<CarImage>(_carImageDal.Get(p => p.Id == id));
        }
        [CacheAspect]
        public IDataResult<List<CarImage>> GetAll()
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll());
        }

        public IDataResult<List<CarImage>> GetImagesByCarId(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId);
            IfCarImageOfCarNotExistsAddDefault(result, carId);

            return new SuccessDataResult<List<CarImage>>(result);
        }

        //[SecuredOperation("carimage.update,moderator,admin")]
        [CacheRemoveAspect("ICarImageService.Get")]
        public IResult Update(CarImage carImage, IFormFile file)
        {
            var carImageToUpdate = _carImageDal.Get(c => c.Id == carImage.Id);
            carImage.CarId = carImageToUpdate.CarId;
            carImage.ImagePath = new FileManagerOnDisk().Update(carImageToUpdate.ImagePath, file, CreateNewPath(file));
            carImage.Date = DateTime.Now;
            _carImageDal.Update(carImage);
            return new SuccessResult(Messages.CarImageUpdated);
        }
        private void IfCarImageOfCarNotExistsAddDefault(List<CarImage> result, int carId)
        {
            if (!result.Any())
            {
                var defaultCarImage = new CarImage
                {
                    CarId = carId,
                    ImagePath =
                        $@"{Environment.CurrentDirectory}\Public\Images\CarImage\default-img.png",
                    Date = DateTime.Now
                };
                result.Add(defaultCarImage);
            }
        }

        private string CreateNewPath(IFormFile file)
        {
            var fileInfo = new FileInfo(file.FileName);
            var newPath =
                $@"{Environment.CurrentDirectory}\Public\Images\CarImage\Upload\{Guid.NewGuid()}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Year}{fileInfo.Extension}";

            return newPath;
        }

        private IResult CheckIfCarImageCountOfCarCorrect(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId).Count;
            if (result >= 5) return new ErrorResult(Messages.ImageLimitExceded);
            return new SuccessResult();
        }
    }
}

