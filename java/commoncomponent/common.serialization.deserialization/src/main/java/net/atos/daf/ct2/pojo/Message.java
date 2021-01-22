package net.atos.daf.ct2.pojo;

public class Message<T> {
  private final T value;

  public Message(T value) {
    this.value = value;
  }

  public T get() {
    return value;
  }

}
