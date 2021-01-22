package net.atos.daf.ct2.utils;

public enum Operation {
  READ("r"),
  CREATE("c"),
  UPDATE("u"),
  DELETE("d");

  private final String code;

  Operation(String code) {
    this.code = code;
  }

  public static Operation forCode(String code) {
    Operation[] var1 = values();
    int var2 = var1.length;

    for (int var3 = 0; var3 < var2; ++var3) {
      Operation op = var1[var3];
      if (op.code().equalsIgnoreCase(code)) {
        return op;
      }
    }
    return null;
  }

  public String code() {
    return this.code;
  }
}
