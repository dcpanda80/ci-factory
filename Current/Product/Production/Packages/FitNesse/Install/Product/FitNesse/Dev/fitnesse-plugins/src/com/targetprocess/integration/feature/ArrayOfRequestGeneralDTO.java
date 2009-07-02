
package com.targetprocess.integration.feature;

import java.util.ArrayList;
import java.util.List;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Java class for ArrayOfRequestGeneralDTO complex type.
 * 
 * <p>The following schema fragment specifies the expected content contained within this class.
 * 
 * <pre>
 * &lt;complexType name="ArrayOfRequestGeneralDTO">
 *   &lt;complexContent>
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
 *       &lt;sequence>
 *         &lt;element name="RequestGeneralDTO" type="{http://targetprocess.com}RequestGeneralDTO" maxOccurs="unbounded" minOccurs="0"/>
 *       &lt;/sequence>
 *     &lt;/restriction>
 *   &lt;/complexContent>
 * &lt;/complexType>
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "ArrayOfRequestGeneralDTO", propOrder = {
    "requestGeneralDTO"
})
public class ArrayOfRequestGeneralDTO {

    @XmlElement(name = "RequestGeneralDTO", required = true, nillable = true)
    protected List<RequestGeneralDTO> requestGeneralDTO;

    /**
     * Gets the value of the requestGeneralDTO property.
     * 
     * <p>
     * This accessor method returns a reference to the live list,
     * not a snapshot. Therefore any modification you make to the
     * returned list will be present inside the JAXB object.
     * This is why there is not a <CODE>set</CODE> method for the requestGeneralDTO property.
     * 
     * <p>
     * For example, to add a new item, do as follows:
     * <pre>
     *    getRequestGeneralDTO().add(newItem);
     * </pre>
     * 
     * 
     * <p>
     * Objects of the following type(s) are allowed in the list
     * {@link RequestGeneralDTO }
     * 
     * 
     */
    public List<RequestGeneralDTO> getRequestGeneralDTO() {
        if (requestGeneralDTO == null) {
            requestGeneralDTO = new ArrayList<RequestGeneralDTO>();
        }
        return this.requestGeneralDTO;
    }

}
