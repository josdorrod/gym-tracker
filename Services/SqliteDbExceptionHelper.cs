using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services;
public class SqliteDbExceptionHelper : IDbExceptionHelper
{
    public bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is not Microsoft.Data.Sqlite.SqliteException sqliteEx)
        {
            return false;
        }
        return sqliteEx.SqliteErrorCode == 2067;
    }
}