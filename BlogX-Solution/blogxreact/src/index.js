import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import { AuthProvider } from "context/AuthProvider";

import "assets/css/nucleo-icons.css";
import "assets/scss/blk-design-system-react.scss";
import "assets/demo/demo.css";

import Index from "views/Index.js";

import PostDetails from "./views/examples/PostDetails";
import RegisterPage from "views/examples/RegisterPage";
import LoginPage from "views/examples/LoginPage";
import ProfilePage from "views/examples/ProfilePage";
import RequireAuth from "./components/Auth/RequireAuth";
import AuthNotLogged from "components/Auth/AuthNotLogged";
import Test from "views/IndexSections/Test";
import NewPostForm from "views/examples/NewPostForm";
import EditPostForm from "views/examples/EditPostForm";

const root = ReactDOM.createRoot(document.getElementById("root"));

root.render(
  <BrowserRouter>
    <AuthProvider>
      <Routes>
        <Route path="/test" element={<Test />} />

        <Route element={<AuthNotLogged />}>
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/login" element={<LoginPage />} />
        </Route>
        <Route element={<RequireAuth />}>
          <Route path="/home" element={<Index />} />
          <Route path="/details/:id" element={<PostDetails />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/newPost" element={<NewPostForm />} />
          <Route path="/editPost/:id" element={<EditPostForm />} />
        </Route>
        <Route path="*" element={<Navigate to="/home" replace />} />
      </Routes>
    </AuthProvider>
  </BrowserRouter>
);
