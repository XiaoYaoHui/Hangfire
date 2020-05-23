using System;
using System.Collections.Generic;
using Hangfire;
using Core.Api.Applications;
using Core.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace Core.Api.Controllers
{
    /// <summary>
    /// 学生Api
    /// </summary>
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger _logger;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="studentRepository"></param>
        /// <param name="logger"></param>
        public StudentsController(IStudentRepository studentRepository,ILogger logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }
        /// <summary>
        /// 获取学生列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Student> Get()
        {
            _logger.Information("开始.......");
            BackgroundJob.Enqueue(() => Console.WriteLine("获取学生列表"));
            _logger.Information(JsonConvert.SerializeObject(_studentRepository.List()));
            _logger.Warning("LogWarning");
            _logger.Error("LogError");
            _logger.Debug("LogDebug");
            return _studentRepository.List();
        }

        /// <summary>
        /// 根据ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Student GetById(int id)
        {
            return _studentRepository.Get(id);
        }

        /// <summary>
        /// 新增学生
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Post([FromBody]Student student)
        {
            return _studentRepository.Add(student);
        }

        /// <summary>
        /// 修改学生
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPut]
        public bool Put([FromBody]Student student)
        {
            return _studentRepository.Update(student);
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            return _studentRepository.Delete(id);
        }
    }
}
