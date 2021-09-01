package net.atos.daf.ct2.models;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@ToString
public class Index extends net.atos.daf.ct2.pojo.standard.Index implements Serializable {
	List<Object> list = new ArrayList<>();
	
}
