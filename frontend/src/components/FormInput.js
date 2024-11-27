import React from "react";

const FormInput = ({ label, type, name, value, onChange, placeholder }) => {
    return (
        <div>
            <label>{label}</label>
            <input
                type={type}
                name={name}
                value={value}
                onChange={onChange}
                placeholder={placeholder}
                required
            />
        </div>
    );
};

export default FormInput;