﻿using SampleApi.Elastic.Data.Models;

namespace SampleApi.Elastic.Services.DTO.Response
{
    public class UserResponse
    {
        public List<User> Data { get; set; }
        public int Total { get; set; }

    }
}
