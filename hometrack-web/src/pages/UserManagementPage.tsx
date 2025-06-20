import React from 'react';
import { Link } from 'react-router-dom';
import './UserManagementPage.css';

const UserManagementPage: React.FC = () => {
    const users = [
        { id: 1, name: 'John Doe', email: 'john.doe@example.com', role: 'User', status: 'Active' },
        { id: 2, name: 'Jane Smith', email: 'jane.smith@example.com', role: 'Admin', status: 'Active' },
        { id: 3, name: 'Peter Jones', email: 'peter.jones@example.com', role: 'User', status: 'Inactive' },
        { id: 4, name: 'Alice Brown', email: 'alice.brown@example.com', role: 'User', status: 'Active' },
    ];

    const handleDelete = (id: number) => {
        console.log('Delete user:', id);
        // Implement actual delete logic here
    };

    const handleChangeStatus = (id: number, status: string) => {
        console.log(`Change status for user ${id} to ${status}`);
        // Implement actual status change logic here
    };

    return (
        <div className="user-management-container">
            <aside className="sidebar">
                <div className="logo">
                    <h1>HomeTrack Admin</h1>
                </div>
                <nav className="main-nav">
                    <ul>
                        <li><Link to="/dashboard">Dashboard</Link></li>
                        <li><Link to="/user-management">User Management</Link></li>
                        <li><Link to="/system-settings">System Settings</Link></li>
                        <li><Link to="/monitoring">Monitoring</Link></li>
                        <li><Link to="/content-moderation">Content Moderation</Link></li>
                        <li><Link to="/">Logout</Link></li>
                    </ul>
                </nav>
            </aside>
            <main className="main-content">
                <header className="main-header">
                    <h2>User Management</h2>
                    <div className="user-info">
                        <span>Admin User</span>
                        <button className="logout-button">Logout</button>
                    </div>
                </header>
                <section className="user-list">
                    <h3>All Users</h3>
                    <table>
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Role</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((user) => (
                                <tr key={user.id}>
                                    <td>{user.id}</td>
                                    <td>{user.name}</td>
                                    <td>{user.email}</td>
                                    <td>{user.role}</td>
                                    <td>{user.status}</td>
                                    <td>
                                        <button className="action-button edit-button">Edit</button>
                                        <button
                                            className="action-button delete-button"
                                            onClick={() => handleDelete(user.id)}
                                        >
                                            Delete
                                        </button>
                                        <button
                                            className="action-button status-button"
                                            onClick={() => handleChangeStatus(user.id, user.status === 'Active' ? 'Inactive' : 'Active')}
                                        >
                                            {user.status === 'Active' ? 'Deactivate' : 'Activate'}
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </section>
            </main>
        </div>
    );
};

export default UserManagementPage; 