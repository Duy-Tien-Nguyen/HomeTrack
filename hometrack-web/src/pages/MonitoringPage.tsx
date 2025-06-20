import React from 'react';
import { Link } from 'react-router-dom';
import './MonitoringPage.css';

const MonitoringPage: React.FC = () => {
    const systemLogs = [
        { time: '12:45:32', level: 'ERROR', message: 'Database connection failed - timeout after 30s' },
        { time: '12:42:15', level: 'WARN', message: 'High memory usage detected (85%)' },
        { time: '12:40:03', level: 'INFO', message: 'User authentication successful (ID: 45892)' },
    ];

    const metrics = {
        cpuUsage: '75%',
        memoryUsage: '60%',
        diskSpace: '45% used',
        networkTraffic: '100 Mbps',
        apiResponseTime: '150 ms',
    };

    return (
        <div className="monitoring-container">
            <aside className="sidebar">
                <div className="logo">
                    <h1>Giám sát & thống kê hệ thống</h1>
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
                    <h2>Giám sát & thống kê hệ thống</h2>
                    <div className="header-right">
                        <span>Admin</span>
                        <div className="user-avatar">A</div>
                    </div>
                </header>

                <section className="reports-section">
                    <div className="reports-header">
                        <h3>Báo cáo</h3>
                        <div className="month-filter">
                            <select>
                                <option>Tháng 6, 2023</option>
                                <option>Tháng 5, 2023</option>
                                <option>Tháng 4, 2023</option>
                            </select>
                        </div>
                    </div>
                    <div className="report-cards-grid">
                        <div className="report-card">
                            <p className="report-title">Tổng người dùng</p>
                            <p className="report-value">1,234</p>
                        </div>
                        <div className="report-card">
                            <p className="report-title">Người dùng Premium</p>
                            <p className="report-value">432</p>
                        </div>
                        <div className="report-card">
                            <p className="report-title">Người dùng hoạt động</p>
                            <p className="report-value">876 <span className="trend-icon down">&downarrow;</span></p>
                        </div>
                    </div>
                </section>

                <section className="user-stats-section">
                    <h3>Thống kê người dùng</h3>
                    <div className="user-stats-bar-chart">
                        <div className="stat-row">
                            <span className="stat-label">Miễn phí</span>
                            <div className="progress-bar-container">
                                <div className="progress-bar free" style={{ width: '70%' }}></div>
                            </div>
                            <span className="stat-value">1,245</span>
                        </div>
                        <div className="stat-row">
                            <span className="stat-label">Trả phí</span>
                            <div className="progress-bar-container">
                                <div className="progress-bar paid" style={{ width: '30%' }}></div>
                            </div>
                            <span className="stat-value">458</span>
                        </div>
                    </div>
                    <p className="total-users">Tổng người dùng <span className="total-value">1,830</span></p>
                </section>

                <section className="system-logs-section">
                    <div className="logs-header">
                        <h3>Logs hệ thống</h3>
                        <button className="filter-logs-button">
                            <i className="fas fa-filter"></i> Lọc
                        </button>
                    </div>
                    <div className="logs-table-container">
                        <table>
                            <tbody>
                                {systemLogs.map((log, index) => (
                                    <tr key={index}>
                                        <td className="log-time">{log.time}</td>
                                        <td><span className={`log-level ${log.level.toLowerCase()}`}>{log.level}</span></td>
                                        <td className="log-message">{log.message}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </section>
            </main>
        </div>
    );
};

export default MonitoringPage; 