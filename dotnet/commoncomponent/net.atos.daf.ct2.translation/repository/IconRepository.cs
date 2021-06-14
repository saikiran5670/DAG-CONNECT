using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.translation.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.translation.repository
{
    public class IconRepository : IIconRepository
    {
        private readonly IconCoreMapper _iconCoreMapper;
        private readonly IDataAccess _dataAccess;
        public IconRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _iconCoreMapper = new IconCoreMapper();
        }
        public async Task<string> UpdateIcons(Icon iconlist)
        {
            try
            {
                string is_Result = "";
                var QueryStatement = @"UPDATE master.icon
                                    SET                                
                                    icon=@icon, 
                                    modified_at=@modified_at,
                                    modified_by=@modified_by                                    
                                    WHERE name=@name                                                                 
                                    RETURNING id;";
                int iconId = 0;

                //If name is exist then update
                int name_cnt = await _dataAccess.QuerySingleAsync<int>("select coalesce((SELECT count(*) FROM master.icon where name=@name), 0)", new { name = iconlist.Name });

                if (name_cnt > 0)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@icon", iconlist.Iconn);
                    parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameter.Add("@modified_by", iconlist.ModifiedBy);
                    parameter.Add("@name", iconlist.Name);

                    iconId = await _dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                }
                if (iconId > 0)
                {
                    is_Result = iconlist.Name;
                }

                return is_Result;
            }
            catch (Exception)
            {
                throw;
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
                dynamic icons = await _dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

                List<Icon> iconList = new List<Icon>();
                foreach (dynamic record in icons)
                {
                    iconList.Add(Map(record));
                }
                return iconList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Icon Map(dynamic record)
        {
            Icon entity = new Icon();
            entity.Id = record.id;
            entity.Iconn = record.icon;
            entity.Type = !string.IsNullOrEmpty(record.type) ? Convert.ToString(_iconCoreMapper.MapCharToDTCType(record.type)) : string.Empty;
            entity.WarningClass = record.warning_class;
            entity.WarningNumber = record.warning_number;
            entity.Name = record.name;
            entity.ColorName = !string.IsNullOrEmpty(record.color_name) ? Convert.ToString(_iconCoreMapper.MapCharToColorName(record.color_name)) : string.Empty;
            entity.State = !string.IsNullOrEmpty(record.state) ? Convert.ToString(_iconCoreMapper.MapCharToIconState(record.state)) : string.Empty;
            entity.CreatedAt = record.created_at;
            entity.CreatedBy = record.created_by;
            entity.ModifiedAt = record.modified_at;
            entity.ModifiedBy = record.modified_by;

            return entity;
        }
    }
}
