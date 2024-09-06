import { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function ForgotPassword() {
  const [form, setForm] = useState({});
  const nav = useNavigate();
  const handleInput = (e) => {
    const value = e.target;
    setForm(() => ({ ...form, [value.name]: value.value }));
  };
  return (
    <div className="w-full max-w-lg p-4 mx-auto bg-white rounded-2xl overflow-hidden shadow-md sm:p-6 lg:p-8 relative">
      <button
        className="absolute left-5 top-2 text-2xl font-bold text-gray-500 hover:text-gray-800"
        onClick={() => nav("/")}
      >
        {"<"}
      </button>
      <header className="mb-4">
        <h1 className="text-2xl font-semibold text-center">Reset Password</h1>
      </header>
      <div className="flex flex-col space-y-4">
        <label>Enter the email address you use to sign in.</label>
        <div>
          <label>Email</label>
          <input
            className="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500 mt-2"
            type="text"
            onChange={handleInput}
            name="Email"
            placeholder="Email"
            value={form?.Email || ""}
          />
        </div>
        <button className="w-full bg-blue-500 text-white rounded-lg p-2 hover:bg-blue-600 transition duration-200">
          Get Password Reset Link
        </button>
        <div
          id="buttonDiv"
          className="w-full rounded-lg overflow-hidden flex justify-center"
        ></div>
        <div className="flex justify-center">
          <a href="/signup">Back to Sign In</a>
        </div>
      </div>
    </div>
  );
}
