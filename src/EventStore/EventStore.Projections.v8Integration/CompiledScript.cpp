#include "stdafx.h"
#include "PreludeScope.h"
#include "CompiledScript.h"
#include "EventHandler.h"

#include <string>


using namespace v8;

namespace js1 
{
	CompiledScript::CompiledScript()
	{
	}

	CompiledScript::~CompiledScript() 
	{
		script.Dispose();
		context.Dispose();
		global.Dispose();
		last_exception.Dispose();
	}

	void CompiledScript::isolate_terminate_execution() 
	{
		v8::Isolate* isolate = get_isolate();
		v8::V8::TerminateExecution(isolate);
	}

	void CompiledScript::report_errors(REPORT_ERROR_CALLBACK report_error_callback)
	{
		v8::Isolate* isolate = get_isolate();
		if (v8::V8::IsDead() || v8::V8::IsExecutionTerminating(isolate)) 
		{
			//TODO: define error codes
			report_error_callback(2, NULL);
			return;
		}

		v8::HandleScope handle_scope;
		v8::Context::Scope local(get_context());

		if (!last_exception.IsEmpty()) 
		{
			v8::String::Value error_value(last_exception);
			//TODO: define error codes
			report_error_callback(1, *error_value);
		}
	}

	v8::Persistent<v8::Context> &CompiledScript::get_context()
	{
		return context;
	}

	bool CompiledScript::compile_script(const uint16_t *script_source, const uint16_t *file_name)
	{
		v8::HandleScope handle_scope;
		//TODO: why dispose? do we call caompile_script multiple times?
		script.Dispose();
		script.Clear();

		global = create_global_template();
		if (global.IsEmpty())
			return false;

		context = v8::Context::New(NULL, global);
		v8::Context::Scope scope(context);

		v8::TryCatch try_catch;
		v8::Handle<v8::Script> result = v8::Script::Compile(v8::String::New(script_source), v8::String::New(file_name));
		set_last_error(result.IsEmpty(), try_catch);

		script = v8::Persistent<v8::Script>::New(result);
		return !script.IsEmpty();
	}

	v8::Handle<v8::Value> CompiledScript::run_script(v8::Persistent<v8::Context> context)
	{
		v8::Context::Scope context_scope(context);
		v8::TryCatch try_catch;
		v8::Handle<v8::Value> result = script->Run();
		set_last_error(result.IsEmpty(), try_catch);
		return result;
	}

	void CompiledScript::set_last_error(bool is_error, v8::TryCatch &try_catch)
	{
		if (!is_error && !try_catch.Exception().IsEmpty()) {
			printf("Caught exception which was not indicated as an error");
		}
		if (is_error) 
		{
			Handle<Value> exception = try_catch.Exception();
			last_exception.Dispose();
			last_exception = v8::Persistent<v8::Value>::New(exception);
		}
		else 
		{
			last_exception.Dispose();
			last_exception.Clear();
		}
	}

	void CompiledScript::set_last_error(v8::Handle<v8::String> message)
	{
		Handle<Value> exception = v8::Exception::Error(message);
		last_exception.Dispose();
		last_exception = v8::Persistent<v8::Value>::New(exception);
	}

	void CompiledScript::isolate_add_ref(v8::Isolate * isolate) 
	{
		size_t counter = reinterpret_cast<size_t>(isolate->GetData());
		counter++;
		isolate->SetData(reinterpret_cast<void *>(counter));
	}

	size_t CompiledScript::isolate_release(v8::Isolate * isolate) 
	{
		size_t counter = reinterpret_cast<size_t>(isolate->GetData());
		counter--;
		isolate->SetData(reinterpret_cast<void *>(counter));
		return counter;
	}
}

