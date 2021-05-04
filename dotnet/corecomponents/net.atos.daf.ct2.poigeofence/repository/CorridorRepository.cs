using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.repository
{
   public class CorridorRepository : ICorridorRepository
    {
        private readonly IDataAccess _dataAccess;

        public CorridorRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;


        }



    }
}
