import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import type { AxiosResponse } from 'axios';

import './LoginPage.css';

// Định nghĩa interface cho phản hồi thành công từ API đăng nhập
interface LoginResponse {
    isSuccess: boolean;
    token: string;
    message: string;
    // Thêm các trường khác nếu có trong phản hồi thành công
}

// Định nghĩa lại interface AxiosError nếu không được export trực tiếp
interface CustomAxiosError extends Error {
    response?: AxiosResponse;
    isAxiosError: true; // Thêm thuộc tính này để phù hợp với type guard
    config: Record<string, any>; // Axios config
    code?: string; // Ví dụ: 'ERR_NETWORK', 'ECONNABORTED'
    request?: any; // Request object
}

// Custom type guard cho AxiosError
function isCustomAxiosError(error: unknown): error is CustomAxiosError {
    return (error as CustomAxiosError).isAxiosError === true;
}

const LoginPage: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        try {
            const response = await axios.post<LoginResponse>('http://localhost:5000/api/Auth/login', {
                email: email,
                password: password,
            });

            if (response.data.isSuccess) {
                localStorage.setItem('token', response.data.token);
                console.log('Login successful', response.data);
                navigate('/dashboard');
            } else {
                setError(response.data.message || 'Login failed');
            }
        } catch (err: unknown) {
            if (isCustomAxiosError(err)) {
                setError(err.response?.data?.message || 'Server error');
            } else {
                setError('Could not connect to server');
            }
            console.error('Login error', err);
        }
    };

    return (
        <div className="login-page-container">
            <div className="login-form-container">
                <h2>Đăng nhập quản trị</h2>
                <h3>Admin Login</h3>
                <form onSubmit={handleLogin}>
                    <div className="input-group">
                        <label htmlFor="email">Email</label>
                        <input
                            type="text"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Nhập email của bạn"
                            required
                        />
                    </div>
                    <div className="input-group">
                        <label htmlFor="password">Mật khẩu</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="**********"
                            required
                        />
                    </div>
                    {error && <p className="error-message">{error}</p>}
                    <button type="submit" className="login-button">Đăng nhập</button>
                </form>
                <p className="forgot-password-link">
                    <Link to="/forgot-password">Quên mật khẩu</Link>
                </p>
            </div>
            <div className="background-image-container"></div>
        </div>
    );
};

export default LoginPage; 