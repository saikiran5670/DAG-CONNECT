using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using System.Data;
using System.Linq;
using Dapper;
using net.atos.daf.ct2.translation.entity;
namespace net.atos.daf.ct2.translation.repository
{
    public class IconRepository: IIconRepository
    {
        private readonly IDataAccess dataAccess;
        public IconRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        public async Task<bool> UpdateIcons(List<Icon> iconlist)
        {
            try
            {                 
                bool is_Result=false;
                var QueryStatement=@"UPDATE master.icon
                                    SET                                
                                    icon=@icon, 
                                    modified_at=@modified_at,
                                    modified_by=@modified_by                                    
                                    WHERE name=@name                                                                 
                                    RETURNING id;";
                int iconId=0;                   
                foreach(var icon in iconlist)
                {
                    //If name is exist then update
                    int name_cnt = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT count(*) FROM master.icon where name=@name), false)", new { name = icon.name });
                    
                    if(name_cnt>0)
                    {
                        var parameter = new DynamicParameters();                
                        parameter.Add("@icon", icon.icon);
                        parameter.Add("@modified_at", icon.modified_at);
                        parameter.Add("@modified_by", icon.modified_by);
                        parameter.Add("@name", icon.name); 

                        iconId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter); 
                    } 

                    is_Result=iconId > 0;  
                }

                return is_Result;
            }
            catch(Exception ex)
            {
                throw ex;
            }            
        }

        public async Task<List<Icon>> GetIcons(int icon_id)
        {
            try
            {
                var QueryStatement = @"select 
                                        id, 
                                        icon, 
                                        type, 
                                        warning_class, 
                                        warning_number, 
                                        name, 
                                        color_name, 
                                        state, 
                                        created_at, 
                                        created_by, 
                                        modified_at, 
                                        modified_by
                                        from master.icon 
                                        where id=@id";
                var parameter = new DynamicParameters();
            
                parameter.Add("@id", icon_id);
                dynamic icons = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                List<Icon> iconList = new List<Icon>();
                foreach (dynamic record in icons)
                {                    
                    iconList.Add(Map(record));
                }
                return iconList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private Icon Map(dynamic record)
        {
            Icon entity = new Icon();
             entity.id =record.id;
             entity.icon =record.icon;
             entity.type =record.type;
             entity.warning_class=record.warning_class;
             entity.warning_number =record.warning_number;
             entity.name =record.name;
             entity.color_name =record.color_name;
             entity.state =record.state;
             entity.created_at =record.created_at;
             entity.created_by =record.created_by;
             entity.modified_at=record.modified_at;
             entity.modified_by=record.modified_by;
           
            return entity;
        }
    }
} 
