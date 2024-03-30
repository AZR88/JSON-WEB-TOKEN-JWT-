using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PercobaanApi1.Helpers;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PercobaanApi1.Models
{
    public class LoginContext
    {
        private string __constr;
        private string __errormsg;

        public LoginContext(string pConstr)
        {
            __constr = pConstr;
        }

        public bool IsValidUser(string p_username, string p_password)
        {
            string query = string.Format(@"SELECT COUNT(*)
        FROM users.person ps
        INNER JOIN users.peran_person pp ON ps.id_person=pp.id_person
        INNER JOIN users.peran p ON pp.id_peran=p.id_peran
        where ps.username='{0}' and ps.password='{1}' and pp.id_peran=1; ", p_username, p_password);
            SqlDBHelper db = new SqlDBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                int count = int.Parse(cmd.ExecuteScalar().ToString());

                cmd.Dispose();
                db.closeConnection();

                return count > 0;
            }
            catch (Exception ex)
            {
                __errormsg = ex.Message;
                return false;
            }
        }

        public string GenerateJwtToken(string namaUser, IConfiguration pConfig)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(pConfig["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, namaUser),
        new Claim(ClaimTypes.Role, "1"),
      };
            var token = new JwtSecurityToken(pConfig["Jwt:Issuer"],
              pConfig["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
