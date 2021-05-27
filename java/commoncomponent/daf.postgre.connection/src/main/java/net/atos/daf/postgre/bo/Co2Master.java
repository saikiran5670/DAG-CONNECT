package net.atos.daf.postgre.bo;

import java.io.Serializable;

public class Co2Master implements Serializable {
	private double coefficient;
	/*
	 * private double coefficient_B; public double getCoefficient_D() { return
	 * coefficient_D; } public void setCoefficient_D(double coefficient_D) {
	 * this.coefficient_D = coefficient_D; } public double getCoefficient_B() {
	 * return coefficient_B; } public void setCoefficient_B(double coefficient_B) {
	 * this.coefficient_B = coefficient_B; }
	 */

	public double getCoefficient() {
		return coefficient;
	}

	public void setCoefficient(double coefficient) {
		this.coefficient = coefficient;
	}
	

}
