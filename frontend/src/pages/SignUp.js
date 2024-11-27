import React, { useState } from "react";
import { Form, Button, Alert } from "react-bootstrap";
import { registerUser } from "../services/apiService";
import "./Login.css";

const SignUp = () => {
    const [formData, setFormData] = useState({
        name: "",
        userName: "", // Matches the input name for "Username"
        email: "",
        password: "",
        confirmPassword: "",
        role: "user", // Default to "user"
    });
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleSignUp = async (e) => {
        e.preventDefault();
        setError(null);
        setSuccess(false);

        if (!formData.name) {
            setError("Name is required");
            return;
        }

        if (formData.password !== formData.confirmPassword) {
            setError("Passwords do not match");
            return;
        }

        console.log("Payload being sent:", {
            name: formData.name,
            username: formData.userName, // Changed to match backend DTO property
            email: formData.email,
            password: formData.password,
            role: formData.role,
        });

        try {
            const response = await registerUser({
                name: formData.name,
                username: formData.userName, // Changed to match backend DTO property
                email: formData.email,
                password: formData.password,
                role: formData.role,
            });

            console.log("Registration successful:", response);
            setSuccess(true);
        } catch (err) {
            setError(err.message);
        }
    };


    return (
        <Form className="login-form" onSubmit={handleSignUp}>
            {error && <Alert variant="danger">{error}</Alert>}
            {success && <Alert variant="success">Registration successful!</Alert>}

            <Form.Label className="login-label">Name:</Form.Label>
            <Form.Control
                type="text"
                placeholder="Enter your full name"
                className="login-input"
                name="name"
                value={formData.name}
                onChange={handleChange}
            />
            <Form.Label className="login-label">Username:</Form.Label>
            <Form.Control
                type="text"
                placeholder="Enter username"
                className="login-input"
                name="userName" // Changed to match formData key
                value={formData.userName}
                onChange={handleChange}
            />
            <Form.Label className="login-label">Email:</Form.Label>
            <Form.Control
                type="email"
                placeholder="Enter email"
                className="login-input"
                name="email"
                value={formData.email}
                onChange={handleChange}
            />
            <Form.Label className="login-label">Password:</Form.Label>
            <Form.Control
                type="password"
                placeholder="Enter password"
                className="login-input"
                name="password"
                value={formData.password}
                onChange={handleChange}
            />
            <Form.Label className="login-label">Confirm Password:</Form.Label>
            <Form.Control
                type="password"
                placeholder="Confirm password"
                className="login-input"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
            />
            <Button variant="primary" type="submit" className="login-submit">
                Sign Up
            </Button>
        </Form>
    );
};

export default SignUp;
