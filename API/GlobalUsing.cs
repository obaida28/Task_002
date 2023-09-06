 global using API.ErrorResponse;
 global using API.DTOs;
 global using API.Helpers;
 global using API.ActionFilter;
 global using API.Validation;
 global using API.ServiceExtension;
 global using API.Middlewares;

 global using Core.Entites;
 global using Core.Interfaces;

 global using Infrastructure.Data;
 global using Infrastructure.Data.UnitOfWork;

 global using Microsoft.EntityFrameworkCore;
 global using Microsoft.AspNetCore.Mvc;
 global using Microsoft.AspNetCore.Mvc.Filters;
 global using Microsoft.AspNetCore.Mvc.ModelBinding;

 global using AutoMapper;

 global using System.ComponentModel;
 global using System.ComponentModel.DataAnnotations;
 global using System.Text.Json.Serialization;
 global using System.Linq;

 global using Newtonsoft.Json;

 global using System.Net;
 global using Microsoft.AspNetCore.Mvc.ApiExplorer;
 global using System.ComponentModel.DataAnnotations.Schema;

global using System.Text;
global using Infrastructure.Services;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.Extensions.Caching.Memory;