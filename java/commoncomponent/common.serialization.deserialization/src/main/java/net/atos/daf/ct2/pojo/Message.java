package net.atos.daf.ct2.pojo;

import java.util.Objects;

public class Message<T> {
  private final T value;

  public Message(T value) {
    this.value = value;
  }

  public T get() {
    return value;
  }

@Override
public int hashCode() {
	return Objects.hash(value);
}

@Override
public boolean equals(Object obj) {
	if (this == obj)
		return true;
	if (obj == null)
		return false;
	if (getClass() != obj.getClass())
		return false;
	Message other = (Message) obj;
	return Objects.equals(value, other.value);
}
  
  

}
