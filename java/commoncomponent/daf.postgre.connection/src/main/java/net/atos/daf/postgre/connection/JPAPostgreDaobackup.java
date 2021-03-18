package net.atos.daf.postgre.connection;


import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.IntStream;

import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;
import javax.persistence.EntityTransaction;
import javax.persistence.Persistence;




public class JPAPostgreDaobackup<T> {

	
	private static JPAPostgreDaobackup jpaPostgreDao;
	private EntityManagerFactory factory;
	private EntityManager entityManager;
	
	//private String password ="W%PQ1AI}Y97";
	//private static String url ="jdbc:postgresql://dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com:5432/Test?sslmode=require";
	// private List<TripSink> tripMessageList = new ArrayList<TripSink>();
	//private List<IndexMsgData> tripMessageList = new ArrayList<IndexMsgData>();

	private JPAPostgreDaobackup() {

	}

	
	public static JPAPostgreDaobackup getInstance(String url, String password) {
		
		if (jpaPostgreDao == null) {
			System.out.println("taking connection");
			synchronized (JPAPostgreDaobackup.class) {
				if (jpaPostgreDao == null) {
					System.out.println("before calling entity manager");
					// if instance is null, initialize
					jpaPostgreDao = new JPAPostgreDaobackup();
					//jpaPostgreDao.createEntityManager(url,password);
				}
				jpaPostgreDao.createEntityManager(url,password);
			}
		}
		return jpaPostgreDao;
	}
	
	

	private void createEntityManager(String url, String Password) {
		for (int i=0;i<=2;i++){
			try {
				System.out.println("inside first line of creteEntityManger");
				//factory = Persistence.createEntityManagerFactory("TestJPA45");//getProperties() 
				factory = Persistence.createEntityManagerFactory("TestJPA45", getProperties(url,Password));
				System.out.println("factory created");
				entityManager = factory.createEntityManager();
				System.out.println("entityManger created");
				break;
			} catch (Exception e) {
				e.printStackTrace();
				// TODO: handle exception
			}
		}
		
	}

/*	public void addTripMessage(IndexMsgData message) {
		tripMessageList.add(message);
		if (tripMessageList.size() == 5) {
			saveTripDetails(tripMessageList);
		}
	}*/

	public void saveTripDetails(List<T> tripMessageListToSave) {
		System.out.println("Inside savetripDetails");
		System.out.println("entityManager==" + entityManager);
		if(entityManager!=null){
			EntityTransaction transaction = entityManager.getTransaction();
			transaction.begin();
			//System.out.println("anshu2");
			for (T insertData : tripMessageListToSave) {
				System.out.println("inside for loop" + insertData.toString() );
				entityManager.merge(insertData);
			}
			entityManager.flush();
			transaction.commit();
			//entityManager.clear();
		}
		
		
	}
	
	private Map<String, String> getProperties(String url, String password) {
	    Map<String, String> result = new HashMap<String, String>();

	    // Read the properties from a file instead of hard-coding it here.
	    // Or pass the password in from the command-line.
	    result.put( "javax.persistence.jdbc.password", password );
	    result.put("javax.persistence.jdbc.url", url);

	    return result;
	  }

	/*
	 * public void save(T t) { // if (factory == null) { //
	 * createEntityManager(); // } EntityTransaction transaction =
	 * entityManager.getTransaction(); transaction.begin();
	 * entityManager.merge(t); entityManager.getTransaction().commit();
	 * //entityManager.close(); }
	 */

	public void close() {
		entityManager.close();
		factory.close();
	}

}
