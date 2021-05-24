/**
 * 
 */
package utiliities;

import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;
import org.json.simple.parser.ParseException;

import com.github.wnameless.json.flattener.JsonFlattener;

import objectProperties.Constants;

/**
 * @author A718166
 *
 */
public class temp {

	/**
	 * @param args
	 * @throws ParseException 
	 * @throws IOException 
	 * @throws FileNotFoundException 
	 */
	public static void main(String[] args) throws FileNotFoundException, IOException, ParseException {
		// TODO Auto-generated method stub
		String localDir = System.getProperty("user.dir");
		String path =localDir + Constants.Json_Path;
		String temp;
		JSONParser parsera = new JSONParser();
		Object objOrg = parsera.parse(new FileReader(path+ "Subscription_Org1.json"));
        JSONObject jsonObjectOrg = (JSONObject)objOrg;//"organizationID"
        Map<String, Object> flattenedJsonMap =JsonFlattener.flattenAsMap(jsonObjectOrg.toString());
        Map<String, Object> items =flattenedJsonMap;            		
    	String OrgID = items.getOrDefault("SubscribeEvent.organizationID", "No vaLue").toString();
        System.out.println(items.getOrDefault("SubscribeEvent.organizationID", "No vaLue"));
      
        
		JSONParser parser = new JSONParser();
		Object obj = parser.parse(new FileReader(path+ "Unsubscription_Org1.json"));
        JSONObject jsonObject = (JSONObject)obj;
    	System.out.println(jsonObject.toString());
    	JSONObject updated_jsonObject =replacekeyInJSONObject(jsonObject,"orderID", "UpdatedORderID");
    	JSONObject updated_jsonObject1 =replacekeyInJSONObject(updated_jsonObject,"organizationID", OrgID);
    	System.out.println(updated_jsonObject1.toString());

    	
		// write to output file
//		try (Writer out = new FileWriter("output.json")) {
//		    out.write(value.toJSONString());
//		}
	}
	private static JSONObject replacekeyInJSONObject(JSONObject jsonObject, String jsonKey, String jsonValue) {

        for (Object key : jsonObject.keySet()) {
            if (key.equals(jsonKey) && ((jsonObject.get(key) instanceof String)||(jsonObject.get(key) instanceof Number)||jsonObject.get(key) ==null)) {
                jsonObject.put(key, jsonValue);
                return jsonObject;
            } else if (jsonObject.get(key) instanceof JSONObject) {
                JSONObject modifiedJsonobject = (JSONObject) jsonObject.get(key);
                if (modifiedJsonobject != null) {
                    replacekeyInJSONObject(modifiedJsonobject, jsonKey, jsonValue);
                }
            }

        }
        return jsonObject;
    }

}
