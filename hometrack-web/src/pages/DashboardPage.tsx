import React from 'react';
import { Link } from 'react-router-dom';
import './DashboardPage.css';

const DashboardPage: React.FC = () => {
    return (
        <div className="dashboard-container">
            <aside className="sidebar">
                <div className="logo">
                    <h1>Dashboard</h1>
                </div>
                <nav className="main-nav">
                    <ul>
                        <li><Link to="/user-management">Quản lý người dùng</Link></li>
                        <li><Link to="/system-settings">Cấu hình hệ thống</Link></li>
                        <li><Link to="/monitoring">Giám sát & thống kê</Link></li>
                        <li><Link to="/content-moderation">Quản lý nội dung</Link></li>
                        <li><Link to="/">Logout</Link></li>
                    </ul>
                </nav>
            </aside>
            <main className="main-content">
                <header className="main-header">
                    <h2>Tổng quan</h2>
                    <span className="welcome-message">Chào mừng trở lại, Admin!</span>
                    <div className="header-right">
                        <div className="search-box">
                            <input type="text" placeholder="Tìm kiếm..." />
                            <i className="search-icon">🔍</i>
                        </div>
                        <div className="user-avatar">A</div>
                    </div>
                </header>

                <section className="dashboard-widgets">
                    <div className="widget">
                        <h3>Tổng người dùng</h3>
                        <p className="metric-value">2,543</p>
                    </div>
                    <div className="widget">
                        <h3>Doanh thu</h3>
                        <p className="metric-value">₫45.2M</p>
                    </div>
                    <div className="widget">
                        <h3>Đơn hàng</h3>
                        <p className="metric-value">1,247</p>
                    </div>
                    <div className="widget">
                        <h3>Tăng trưởng</h3>
                        <p className="metric-value growth-positive">+12.5%</p>
                    </div>
                </section>

                <section className="chart-and-activity">
                    <div className="chart-container">
                        <h3>Biểu đồ doanh thu</h3>
                        <p className="chart-placeholder">Biểu đồ sẽ được hiển thị ở đây</p>
                    </div>

                    <div className="recent-activity-section">
                        <h3>Hoạt động gần đây</h3>
                        <ul className="activity-list">
                            <li><span className="activity-dot new-user"></span> Người dùng mới đăng ký <span className="activity-time">2 phút trước</span></li>
                            <li><span className="activity-dot new-order"></span> Đơn hàng mới <span className="activity-time">5 phút trước</span></li>
                            <li><span className="activity-dot product-update"></span> Cập nhật sản phẩm <span className="activity-time">10 phút trước</span></li>
                        </ul>
                    </div>
                </section>
            </main>
        </div>
    );
};

export default DashboardPage; 