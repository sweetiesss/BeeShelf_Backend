import React, { useContext, useState } from "react";
import { AuthContext } from "../../setting/AuthContext";

export default function EditProfilePage() {
  const { userInfor, setUserInfor } = useContext(AuthContext); // Access the context to get and update user info
  const [formData, setFormData] = useState({
    name: userInfor?.name || "",
    email: userInfor?.email || "",
    phone: userInfor?.phone || "",
    password: "",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prevState) => ({ ...prevState, [name]: value }));
  };

  const handleSave = () => {
    // Validate and save the form data (e.g., send it to the backend)
    console.log("Saved:", formData);
    setUserInfor({ ...userInfor, ...formData }); // Update user information in context
  };

  const handleReset = () => {
    // Reset the form to its original state
    setFormData({
      name: userInfor?.name || "",
      email: userInfor?.email || "",
      phone: userInfor?.phone || "",
      password: "",
    });
  };

  return (
    <div className="w-full max-w-4xl mx-auto p-6 bg-white shadow-lg rounded-lg mt-10">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Settings</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Name Field */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">Name</label>
          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Enter your name"
          />
        </div>

        {/* Email Field */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">Email</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Enter your email"
          />
        </div>

        {/* Phone Field */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">Phone</label>
          <input
            type="text"
            name="phone"
            value={formData.phone}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Enter your phone number"
          />
        </div>

        {/* Password Field */}
        <div>
          <label className="block text-gray-700 font-semibold mb-2">Password</label>
          <input
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Enter a new password"
          />
        </div>
      </div>

      {/* Buttons */}
      <div className="flex space-x-4 mt-8">
        <button
          onClick={handleSave}
          className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
        >
          Save Changes
        </button>
        <button
          onClick={handleReset}
          className="bg-gray-500 text-white px-4 py-2 rounded-lg hover:bg-gray-600"
        >
          Reset
        </button>
      </div>
    </div>
  );
}
